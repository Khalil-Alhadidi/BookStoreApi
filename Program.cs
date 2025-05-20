using Serilog;
using BookStoreApi.Features.Books;
using BookStoreApi.Features.Users;
using BookStoreApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using BookStoreApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Load environment-specific configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Serilog configuration
// In Production, you might want to use a more sophisticated logging configuration (e.g., write to files, Serilog.Sinks.ApplicationInsights for Azure etc.)
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "BookStoreApi")
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

// Register services
ServiceRegistration.RegisterServices(builder.Services, builder.Configuration);



var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    // Create all roles
    foreach (var role in new[] { Roles.Admin, Roles.User, Roles.TestRole })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
        }
    }

    // Ensure the admin user exists
    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = "admin",
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(adminUser, "Aa@12340000");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }
        else
        {
            // Log or handle errors
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"Failed to create admin user: {errors}");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// To log requests and responses
app.UseSerilogRequestLogging();

// Important: Add these middleware components in the correct order
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication(); // Add this before UseAuthorization
app.UseAuthorization();

// Global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An unhandled exception occurred while processing the request."+ex.Message);
        context.Response.StatusCode = 500;
        var problemDetails = new ProblemDetails
        {
            Status = 500,
            Title = "An unexpected error occurred.",
            Detail = ex.Message,
            Instance = context.Request.Path
        };
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
});

// Map versioned endpoints
app.MapBookEndpoints();
app.MapAuthEndpoints();



app.MapHealthChecks("/health");

app.Run();
