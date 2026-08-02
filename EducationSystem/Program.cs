using EducationSystem.Infrastructure;

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

#region Middlewares

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion Middlewares

app.Run();