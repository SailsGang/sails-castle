using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Infrastructure.Caching;
using SailsEnergy.Infrastructure.Identity;
using SailsEnergy.Infrastructure.Messaging;
using SailsEnergy.Infrastructure.Services;

namespace SailsEnergy.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("sailsenergy")
                               ?? configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException("Database connection string not found");

        // HTTP Context for current user
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IAuditService, AuditService>();

        // Hybrid Cache (L1 Memory + L2 Redis when configured)
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new()
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            };
        });
        services.AddScoped<ICacheService, HybridCacheService>();

        return services;
    }

    public static IHostApplicationBuilder AddMessaging(
        this IHostApplicationBuilder builder)
    {
        var dbConnectionString = builder.Configuration.GetConnectionString("sailsenergy")
                                 ?? builder.Configuration.GetConnectionString("Database")
                                 ?? throw new InvalidOperationException("Database connection string not found");

        var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq")
                                       ?? builder.Configuration.GetConnectionString("RabbitMQ")
                                       ?? "amqp://sails:sails123@localhost:5672";

        builder.AddWolverineMessaging(dbConnectionString, rabbitMqConnectionString);

        return builder;
    }

    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("sailsenergy")
                               ?? configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException("Database connection string not found");

        // EF Core for Identity
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // JWT
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        return services;
    }
}
