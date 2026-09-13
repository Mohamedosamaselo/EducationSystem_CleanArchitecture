using EducationSystem.Application;
using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure;
using EducationSystem.Infrastructure.Persistence;
using EducationSystem.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);

// Register Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "EducationSystem API",
        Description = "ASP.NET Core Web API for managing the Education System",
        TermsOfService = new Uri("https://example.com/terms"),
        //Contact = new OpenApiContact
        //{
        //    Name = "EducationSystem",
        //    Url = new Uri("https://example.com/contact")
        //},
        //License = new OpenApiLicense
        //{
        //    Name = "Example License",
        //    Url = new Uri("https://example.com/license")
        //}
    });
});

//builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

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