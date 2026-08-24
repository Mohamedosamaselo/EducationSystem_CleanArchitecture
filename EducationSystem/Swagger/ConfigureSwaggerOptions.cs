using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SurveyBasket.Swagger;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)

            options.SwaggerDoc(
                description.GroupName,
                CreateInfoForApiVersion(description));

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Please Add your token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWt",
            Scheme = "Bearer"
        });

        //        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        //{
        //    {
        //        new OpenApiSecurityScheme
        //        {
        //            Reference = new openApiReference
        //            {
        //                Type = ReferenceType.SecurityScheme,
        //                Id = "Bearer"
        //            }
        //        },
        //        Array.Empty<string>()
        //    }
        //});
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