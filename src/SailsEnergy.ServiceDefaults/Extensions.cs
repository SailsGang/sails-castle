using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace SailsEnergy.ServiceDefaults;

public static class ServiceDefaultsExtensions
{
    private static readonly PathString _healthEndpointPath = new("/health");
    private static readonly PathString _alivenessEndpointPath = new("/alive");

    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        public TBuilder AddServiceDefaults()
        {
            builder.ConfigureOpenTelemetry();
            builder.AddDefaultHealthChecks();
            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();

                http.AddServiceDiscovery();
            });

            return builder;
        }

        public TBuilder ConfigureOpenTelemetry()
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSource(builder.Environment.ApplicationName)
                        .AddAspNetCoreInstrumentation(options =>
                            // Exclude health check requests from tracing
                            options.Filter = context =>
                                !context.Request.Path.StartsWithSegments(_healthEndpointPath, StringComparison.OrdinalIgnoreCase)
                                && !context.Request.Path.StartsWithSegments(_alivenessEndpointPath, StringComparison.OrdinalIgnoreCase)
                        )
                        .AddHttpClientInstrumentation();
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        private TBuilder AddOpenTelemetryExporters()
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

            if (useOtlpExporter)
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            return builder;
        }

        public TBuilder AddDefaultHealthChecks()
        {
            var connectionString = builder.Configuration.GetConnectionString("sailsenergy")
                                   ?? builder.Configuration.GetConnectionString("Database");
            
            var rabbitMqConnectionString = builder.Configuration.GetConnectionString("messaging")
                                           ?? builder.Configuration.GetConnectionString("RabbitMQ");

            var healthChecksBuilder = builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            if (!string.IsNullOrEmpty(connectionString)) 
                healthChecksBuilder.AddNpgSql(connectionString, name: "postgresql", tags: ["db", "ready"]);
            
            if (!string.IsNullOrEmpty(rabbitMqConnectionString))
            {
                healthChecksBuilder.AddRabbitMQ(sp => new RabbitMQ.Client.ConnectionFactory
                {
                    Uri = new Uri(rabbitMqConnectionString)
                }.CreateConnectionAsync().GetAwaiter().GetResult(), name: "rabbitmq", tags: ["messaging", "ready"]);
            }

            return builder;
        }
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (!app.Environment.IsDevelopment()) return app;

        app.MapHealthChecks(_healthEndpointPath);

        app.MapHealthChecks(_alivenessEndpointPath, new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        return app;
    }
}
