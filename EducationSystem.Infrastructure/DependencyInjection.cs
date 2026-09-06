using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Auth;
using EducationSystem.Application.Services;
using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Identity;
using EducationSystem.Infrastructure.Persistence;
using EducationSystem.Infrastructure.Repositories;
using EducationSystem.Infrastructure.unitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EducationSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddDatabase(services, configuration);

        AddIdentity(services);

        AddIdentityOptions(services);

        AddJwtAuthentication(services, configuration);

        AddApplicationServices(services);

        AddRepositories(services);

        AddUnitOfWork(services);

        return services;
    }

    // Database

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
    }

    // Identity

    private static void AddIdentity(IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }

    // Identity Options
    private static void AddIdentityOptions(IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            // Password
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            // User
            options.User.RequireUniqueEmail = true;

            // Sign In
            options.SignIn.RequireConfirmedEmail = true;

            // Lockout
            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(5);

            options.Lockout.MaxFailedAccessAttempts = 5;

            options.Lockout.AllowedForNewUsers = true;
        });
    }

    // JWT Authentication
    private static void AddJwtAuthentication(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSetting>(
            configuration.GetSection("Jwt"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer =
                        configuration["JWT:Issuer"],

                    ValidAudience =
                        configuration["JWT:Audience"],

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                configuration["JWT:Key"]!))
                };
        });
    }

    // Application / Infrastructure Services

    private static void AddApplicationServices(
        IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISchoolService, SchoolService>();
    }

    // Repositories

    private static void AddRepositories(
        IServiceCollection services)
    {
        services.AddScoped(
     typeof(IGenericRepository<>),
            typeof(GenericRepository<>));
    }

    // Unit Of Work

    private static void AddUnitOfWork(
        IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}