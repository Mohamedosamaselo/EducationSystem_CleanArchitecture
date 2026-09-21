using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi; // <--- CHANGED: This is the new namespace for .NET 10
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic; // Needed for List<string>
using Microsoft.OpenApi.Models; // <--- THIS IS REQUIRED

namespace SurveyBasket.Swagger;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateInfoForApiVersion(description));
        }

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Please Add your token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        // THIS IS THE STANDARD CODE THAT WORKS IN ALL .NET VERSIONS
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });
    }

    private static OpenApiInfo CreateInfoForApiVersion(
        ApiVersionDescription description) =>
        new()
        {
            Title = "Survey Basket API",
            Version = description.ApiVersion.ToString(),
            Description =
                $"API Description.{(description.IsDeprecated
                    ? " This API version has been deprecated."
                    : string.Empty)}"
        };
}