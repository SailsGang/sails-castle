using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// OpenAPI/Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

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

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithTags("Health");

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
