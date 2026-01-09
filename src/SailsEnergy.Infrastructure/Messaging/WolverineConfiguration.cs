using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SailsEnergy.Application;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Infrastructure.Data;
using Wolverine;
using Wolverine.RabbitMQ;

namespace SailsEnergy.Infrastructure.Messaging;

public static class WolverineConfiguration
{
    public static IHostApplicationBuilder AddWolverineMessaging(
        this IHostApplicationBuilder builder,
        string dbConnectionString,
        string rabbitMqConnectionString)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(dbConnectionString));

        builder.Services.AddScoped<IAppDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());

        builder.UseWolverine(options =>
        {
            options.UseRabbitMq(new Uri(rabbitMqConnectionString))
                .AutoProvision();

            options.ListenToRabbitQueue("sails-energy");
            options.PublishAllMessages().ToRabbitQueue("sails-energy");

            options.Discovery.IncludeAssembly(typeof(ApplicationMarker).Assembly);
        });

        return builder;
    }
}
