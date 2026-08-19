using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure;
using EducationSystem.Infrastructure.Persistence;
using EducationSystem.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration); // Add Configurations For Infrastructure Layer

// Register Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion Configure Services

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<ApplicationDbContext>();

    var roleManager =
        services.GetRequiredService<RoleManager<Role>>();

    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

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