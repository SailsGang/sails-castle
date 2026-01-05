using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Infrastructure.Messaging;
using SailsEnergy.Infrastructure.Persistence.Marten;
using SailsEnergy.Infrastructure.Services;

namespace SailsEnergy.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException("Database connection string not found");

        services.AddMartenConfiguration(connectionString);

        // HTTP Context for current user
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    public static IHostApplicationBuilder AddMessaging(
        this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMQ")
                                       ?? "amqp://sails:sails123@localhost:5672";

        builder.AddWolverineMessaging(rabbitMqConnectionString);

        return builder;
    }
}
