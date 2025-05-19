using BookStoreApi.Features.Books;
using BookStoreApi.Features.Users;
using BookStoreApi.Infrastructure.Data;
using BookStoreApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookStoreApi.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register services
        services.AddScoped<BookStoreApi.Features.Books.IBookService, BookStoreApi.Features.Books.BookService>();
        services.AddScoped<BookStoreApi.Features.Users.IUserService, BookStoreApi.Features.Users.UserService>();
        services.AddScoped<IEmailSender, EmailSender>();


        services.AddHealthChecks();

        services.AddDbContext<BookStoreDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<BookStoreDbContext>()
        .AddDefaultTokenProviders();




        // Configure JWT Authentication
        var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
        var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
        var jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
        var key = Encoding.UTF8.GetBytes(jwtKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });






        return services;
    }
} 