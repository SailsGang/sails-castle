using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using SailsEnergy.Api.Endpoints;
using SailsEnergy.Api.Extensions;
using SailsEnergy.Api.Middleware;
using SailsEnergy.Application;
using SailsEnergy.Infrastructure;
using SailsEnergy.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, resilience)
builder.AddServiceDefaults();

// Add Infrastructure (Marten, etc.)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.AddMessaging();

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<SailsEnergy.Api.Handlers.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Rate limiting for auth endpoints
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
        opt.QueueLimit = 0;
    });
});

builder.Services.AddValidatorsFromAssemblyContaining<ApplicationMarker>();

// OpenAPI
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc, context, ct) =>
    {
        doc.Info.Title = "SailsEnergy API";
        doc.Info.Version = "v1";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.UseRateLimiter();

// Seed database
if (app.Environment.IsDevelopment())
{
    await SailsEnergy.Infrastructure.Identity.DatabaseSeeder.SeedAsync(app.Services);
}

app.UseExceptionHandler();
app.UseCors("Frontend");
app.UseRequestLogging();

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("SailsEnergy API")
               .WithTheme(ScalarTheme.Kepler)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
               .WithPreferredScheme("Bearer");
    });
}

// Endpoints
app.MapAuthEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    Name = "SailsEnergy API",
    Version = "1.0.0",
    Documentation = "/scalar/v1"
}))
.WithName("Root")
.ExcludeFromDescription();

await app.RunAsync();
