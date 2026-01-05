using JasperFx;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Infrastructure.Persistence.Marten;

public static class MartenConfiguration
{
    public static IServiceCollection AddMartenConfiguration(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddMarten(options =>
        {
            options.Connection(connectionString);

            // Auto-create schema in development
            options.AutoCreateSchemaObjects = AutoCreate.All;

            // Register all documents
            options.Schema.For<Gang>().Identity(x => x.Id);
            options.Schema.For<Car>().Identity(x => x.Id);
            options.Schema.For<UserProfile>().Identity(x => x.Id);
            options.Schema.For<Period>().Identity(x => x.Id);
            options.Schema.For<Tariff>().Identity(x => x.Id);
            options.Schema.For<EnergyLog>().Identity(x => x.Id);
            options.Schema.For<GangMember>().Identity(x => x.Id);
            options.Schema.For<GangCar>().Identity(x => x.Id);

            // Indexes for common queries
            options.Schema.For<Gang>()
                .Index(x => x.OwnerId);

            options.Schema.For<GangMember>()
                .Index(x => x.GangId)
                .Index(x => x.UserId);

            options.Schema.For<Period>()
                .Index(x => x.GangId);

            options.Schema.For<EnergyLog>()
                .Index(x => x.PeriodId)
                .Index(x => x.GangId);

            options.Schema.For<Tariff>()
                .Index(x => x.GangId);
        }).UseLightweightSessions();

        return services;
    }
}
