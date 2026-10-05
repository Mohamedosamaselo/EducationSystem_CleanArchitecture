using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;

namespace EducationSystem.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IOrganisationService, OrganisationService>();

        services.AddScoped<IGradeService, GradeServices>();

        services.AddScoped<ISchoolService, SchoolService>();

        services.AddScoped<ISubjectService, SubjectService>();

        //services.AddScoped<IEmailSender, EmailSender>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddFluentValidationAutoValidation();

        services.AddMailKit(optionBuilder =>
        {
            optionBuilder.UseMailKit(new MailKitOptions
            {
                Server = configuration["Email:Server"]!,
                Port = Convert.ToInt32(configuration["Email:Port"]),
                SenderName = configuration["Email:SenderName"]!,
                SenderEmail = configuration["Email:SenderEmail"]!,
                Account = configuration["Email:Account"]!,
                Password = configuration["Email:Password"]!,
                Security = true
            });
        });

        services.AddScoped<IEmailService, MailKitEmailService>();

        return services;
    }
}