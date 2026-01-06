using JasperFx;
using Marten;
using Microsoft.Extensions.Hosting;
using SailsEnergy.Application;
using SailsEnergy.Domain.Entities;
using Wolverine;
using Wolverine.Marten;
using Wolverine.RabbitMQ;
using Weasel.Core;

namespace SailsEnergy.Infrastructure.Messaging;

public static class WolverineConfiguration
{
    public static IHostApplicationBuilder AddWolverineMessaging(
        this IHostApplicationBuilder builder,
        string dbConnectionString,
        string rabbitMqConnectionString)
    {
        builder.UseWolverine(options =>
        {
            options.Services.AddMarten(opts =>
            {
                opts.Connection(dbConnectionString);
                opts.AutoCreateSchemaObjects = AutoCreate.All;

                opts.UseNewtonsoftForSerialization(EnumStorage.AsString, Casing.CamelCase);

                // Document registrations
                opts.Schema.For<Gang>().Identity(x => x.Id);
                opts.Schema.For<Car>().Identity(x => x.Id);
                opts.Schema.For<UserProfile>().Identity(x => x.Id);
                opts.Schema.For<Period>().Identity(x => x.Id);
                opts.Schema.For<Tariff>().Identity(x => x.Id);
                opts.Schema.For<EnergyLog>().Identity(x => x.Id);
                opts.Schema.For<GangMember>().Identity(x => x.Id);
                opts.Schema.For<GangCar>().Identity(x => x.Id);

                // Indexes
                opts.Schema.For<Gang>().Index(x => x.OwnerId);
                opts.Schema.For<GangMember>().Index(x => x.GangId).Index(x => x.UserId);
                opts.Schema.For<Period>().Index(x => x.GangId);
                opts.Schema.For<EnergyLog>().Index(x => x.PeriodId).Index(x => x.GangId);
                opts.Schema.For<Tariff>().Index(x => x.GangId);
            })
            .UseLightweightSessions()
            .IntegrateWithWolverine();

            options.UseRabbitMq(new Uri(rabbitMqConnectionString))
                .AutoProvision();

            options.ListenToRabbitQueue("sails-energy");
            options.PublishAllMessages().ToRabbitQueue("sails-energy");

            options.Discovery.IncludeAssembly(typeof(ApplicationMarker).Assembly);
        });

        return builder;
    }
}

