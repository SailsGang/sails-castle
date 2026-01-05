using SailsEnergy.Infrastructure;
using SailsEnergy.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, resilience)
builder.AddServiceDefaults();

// Add Infrastructure (Marten, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// OpenAPI/Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

// Map Aspire default endpoints (health checks)
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("SailsEnergy API")
               .WithTheme(ScalarTheme.BluePlanet);
    });
}

// Root endpoint
app.MapGet("/", () => Results.Ok(new
{
    Name = "SailsEnergy API",
    Version = "1.0.0",
    Documentation = "/scalar/v1"
}))
.WithName("Root")
.ExcludeFromDescription();

app.Run();
