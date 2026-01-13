using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SailsEnergy.Api.Endpoints;
using SailsEnergy.Api.Extensions;
using SailsEnergy.Api.Hubs;
using SailsEnergy.Api.Middleware;
using SailsEnergy.Application;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Infrastructure;
using SailsEnergy.Infrastructure.Services;
using SailsEnergy.ServiceDefaults;
using Scalar.AspNetCore;
using Asp.Versioning;

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

// API Versioning - header-based with v1 default
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});

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

    options.AddFixedWindowLimiter("energy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 30;
        opt.QueueLimit = 2;
    });

    options.AddFixedWindowLimiter("api", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 0;
    });

    // Rate limit period closing - prevents spam closing periods
    options.AddSlidingWindowLimiter("period-close", opt =>
    {
        opt.Window = TimeSpan.FromHours(1);
        opt.PermitLimit = 2;
        opt.SegmentsPerWindow = 4;
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

builder.Services.AddSignalR();
builder.Services.AddScoped<IRealtimeNotificationService, SignalRNotificationService<NotificationHub>>();

// Response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

app.UseRateLimiter();

// Apply EF Core migrations and seed database
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    // Migrate Identity database first
    var identityDb = scope.ServiceProvider.GetRequiredService<SailsEnergy.Infrastructure.Identity.ApplicationDbContext>();
    await identityDb.Database.MigrateAsync();

    // Migrate App database
    var appDb = scope.ServiceProvider.GetRequiredService<SailsEnergy.Infrastructure.Data.AppDbContext>();
    await appDb.Database.MigrateAsync();

    await SailsEnergy.Infrastructure.Identity.DatabaseSeeder.SeedAsync(app.Services);
}

app.UseExceptionHandler();

// HTTPS and HSTS for production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseResponseCompression();
app.UseCors("Frontend");
app.UseSecurityHeaders();
app.UseRequestLogging();

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/hubs/notifications");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "SailsEnergy API";
        options.Theme = ScalarTheme.Moon;
        options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecurityScheme = "Bearer"
        };
    });
}

// Endpoints
app.MapAuthEndpoints();
app.MapGangEndpoints();
app.MapCarEndpoints();
app.MapUserEndpoints();
app.MapEnergyEndpoints();
app.MapPeriodEndpoints();
app.MapTariffEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    Name = "SailsEnergy API",
    Version = "1.0.0",
    Documentation = "/scalar/v1"
}))
.WithName("Root")
.ExcludeFromDescription();

await app.RunAsync();
