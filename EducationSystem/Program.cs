using Asp.Versioning;
using EducationSystem.Application;
using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure;
using EducationSystem.Infrastructure.Persistence;
using EducationSystem.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SurveyBasket.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);

// 1. Configure API Versioning (THIS IS REQUIRED TO FIX YOUR ERROR)
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// 2. Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Register the custom Swagger Options class
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

#endregion Configure Services

var app = builder.Build();

// Seeding configs
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await DataSeeder.SeedAsync(context, userManager, roleManager);
}

#region Middlewares

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#endregion Middlewares

app.Run();