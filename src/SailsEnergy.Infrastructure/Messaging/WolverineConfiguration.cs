using Marten;
using Microsoft.Extensions.Hosting;
using SailsEnergy.Application;
using Wolverine;
using Wolverine.Marten;
using Wolverine.RabbitMQ;

namespace SailsEnergy.Infrastructure.Messaging;

public static class WolverineConfiguration
{
    public static IHostApplicationBuilder AddWolverineMessaging(
        this IHostApplicationBuilder builder,
        string rabbitMqConnectionString)
    {
        builder.UseWolverine(options =>
        {
            options.Services.AddMarten(opts =>
            {
            }).IntegrateWithWolverine();

            options.UseRabbitMq(new Uri(rabbitMqConnectionString))
                .AutoProvision();

            options.ListenToRabbitQueue("sails-energy");

            options.PublishAllMessages().ToRabbitQueue("sails-energy");

            options.Discovery.IncludeAssembly(typeof(ApplicationMarker).Assembly);
        });

        return builder;
    }
}
