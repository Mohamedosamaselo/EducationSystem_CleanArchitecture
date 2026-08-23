using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Auth;
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
    public static IServiceCollection AddInfrastructure(this IServiceCollection Services, IConfiguration Configuration)
    {
        // connection Strings
        Services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // Jwt configs
        Services.Configure<JwtSetting>(Configuration.GetSection("Jwt"));

        // Authentication , JWt

        Services.AddIdentity<ApplicationUser, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        // LockOut Configs
        Services.Configure<IdentityOptions>(options =>
        {
            // Default Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        });

        Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.SaveToken = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = Configuration["JWT:Issuer"],
                ValidAudience = Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
            };
        });

        // Services
        Services.AddScoped<IAuthService, AuthService>();

        // Generic Repository

        Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // UnitOfWork
        Services.AddScoped<IUnitOfWork, UnitOfWork>();

        return Services;
    }
}