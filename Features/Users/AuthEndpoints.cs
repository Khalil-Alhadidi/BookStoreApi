using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BookStoreApi.Infrastructure;
using BookStoreApi.Features.Users;

namespace BookStoreApi.Features.Users;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = //app.MapGroup("/api/auth")
             app.MapGroup("/api/v{version:apiVersion}/auth")
            .WithTags("Authentication");

        

        group.MapPost("/register", [Authorize(Roles = "Admin")] async (
            [FromBody] RegisterRequest register,
            UserManager<User> userManager,
            IConfiguration configuration,
            ILogger<Program> logger) =>
        {
            if (await userManager.FindByNameAsync(register.Username) != null)
            {
                return Results.BadRequest(new { message = "Username already exists" });
            }

            var user = new User 
            { 
                UserName = register.Username, 
                Email = register.Email,
                FirstName = "User",
                LastName = "User",
                CreatedAt = DateTime.UtcNow 
            };
            var result = await userManager.CreateAsync(user, register.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, Roles.User);
                return Results.Ok(new { message = "User registered successfully" });
            }

            return Results.BadRequest(new { message = "Registration failed", errors = result.Errors });
        });

        group.MapPost("/login", async (
            [FromBody] LoginRequest login,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ILogger<Program> logger) =>
        {
            var user = await userManager.FindByNameAsync(login.Username);
            if (user == null)
            {
                return Results.BadRequest(new { message = "Invalid username or password" });
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (!result.Succeeded)
            {
                return Results.BadRequest(new { message = "Invalid username or password" });
            }

            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["Jwt:ExpireDays"] ?? "1"));

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"] ?? "BookStoreApi",
                configuration["Jwt:Audience"] ?? "BookStoreApi",
                claims,
                expires: expires,
                signingCredentials: creds
            );

            // Generate refresh token
            var refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh token valid for 7 days
            await userManager.UpdateAsync(user);

            return Results.Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = refreshToken,
                expiration = expires,
                username = user.UserName,
                roles = roles
            });
        });

        group.MapPost("/refresh", async (
            [FromBody] RefreshTokenRequest request,
            UserManager<User> userManager,
            IConfiguration configuration) =>
        {
            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Results.BadRequest(new { message = "Invalid refresh token" });
            }

            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["Jwt:ExpireDays"] ?? "1"));

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"] ?? "BookStoreApi",
                configuration["Jwt:Audience"] ?? "BookStoreApi",
                claims,
                expires: expires,
                signingCredentials: creds
            );

            // Generate new refresh token
            var newRefreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            return Results.Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = newRefreshToken,
                expiration = expires,
                username = user.UserName,
                roles = roles
            });
        });

        // Request password reset
        group.MapPost("/forgot-password", async (
            [FromBody] ForgotPasswordRequest request,
            UserManager<User> userManager,
            IEmailSender emailSender,
            IConfiguration configuration) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Results.Ok(new { message = "If your email is registered, you will receive a password reset link." });
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{configuration["FrontendUrl"]}/reset-password?email={Uri.EscapeDataString(user.Email ?? string.Empty)}&token={Uri.EscapeDataString(token)}";

            var emailBody = $@"
                <h2>Password Reset Request</h2>
                <p>Hello {user.FirstName},</p>
                <p>You have requested to reset your password. Click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you did not request this, please ignore this email.</p>
                <p>This link will expire in 1 hour.</p>";

            await emailSender.SendEmailAsync(
                user.Email ?? throw new InvalidOperationException("User email is null"),
                "Password Reset Request",
                emailBody);

            return Results.Ok(new { message = "If your email is registered, you will receive a password reset link." });
        });

        // Reset password
        group.MapPost("/reset-password", async (
            [FromBody] ResetPasswordRequest request,
            UserManager<User> userManager) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Results.BadRequest(new { message = "Invalid request" });
            }

            var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                return Results.BadRequest(new { message = "Invalid or expired token", errors = result.Errors });
            }

            return Results.Ok(new { message = "Password has been reset successfully" });
        });
    }
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Email, string Password);
public record RefreshTokenRequest(string RefreshToken);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword); 