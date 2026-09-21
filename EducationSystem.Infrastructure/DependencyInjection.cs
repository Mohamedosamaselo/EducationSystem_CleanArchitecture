using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Auth;
using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence;
using EducationSystem.Infrastructure.Repositories;
using EducationSystem.Infrastructure.unitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace EducationSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddDatabase(services, configuration);

        AddAuthConfig(services);

        AddIdentityOptions(services);

        AddJwtAuthentication(services, configuration);

        AddApplicationAndInfrastructureServices(services);

        return services;
    }

    // Database

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .UseLazyLoadingProxies()
                   );
    }

    // Identity

    private static void AddAuthConfig(IServiceCollection services)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();

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
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 3;
            //options.Password.RequiredUniqueChars = false;

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

    private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSetting>(configuration.GetSection("JWT"));

        var x = configuration.GetSection("JWT");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
           {
               o.RequireHttpsMetadata = false;
               o.SaveToken = true;

               o.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,

                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!)),
                   ValidIssuer = configuration["JWT:Issuer"],
                   ValidAudience = configuration["JWT:Audience"],

                   ClockSkew = TimeSpan.Zero,

                   RoleClaimType = ClaimTypes.Role
               };
           });

        services.Configure<IdentityOptions>(options =>
         {
             options.Password.RequiredLength = 8;
             options.User.RequireUniqueEmail = true;// require unique email
         });
    }

    private static void AddApplicationAndInfrastructureServices(
        IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}