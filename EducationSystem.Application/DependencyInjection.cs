using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Services;
using EducationSystem.Infrastructure.Identity;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;

namespace EducationSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IOrganisationService, OrganisationService>();

        services.AddScoped<ISchoolService, SchoolService>();

        // Add FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddFluentValidationAutoValidation();

        return services;
    }
}