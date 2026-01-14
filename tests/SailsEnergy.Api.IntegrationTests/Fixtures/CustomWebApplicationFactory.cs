using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SailsEnergy.Infrastructure.Data;
using SailsEnergy.Infrastructure.Identity;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Wolverine;

namespace SailsEnergy.Api.IntegrationTests.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:18")
        .WithDatabase("sailsenergy_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:4-management-alpine")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();

    public string PostgresConnectionString => _postgresContainer.GetConnectionString();
    public string RabbitMqConnectionString => _rabbitMqContainer.GetConnectionString();

    private bool _databaseInitialized;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // This runs BEFORE any configuration is built
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:sailsenergy"] = PostgresConnectionString,
                ["ConnectionStrings:identity"] = PostgresConnectionString,
                ["ConnectionStrings:Database"] = PostgresConnectionString,
                ["ConnectionStrings:redis"] = "localhost:6379",
                ["ConnectionStrings:rabbitmq"] = RabbitMqConnectionString,
                ["Jwt:Secret"] = "TestSecretKeyThatIsAtLeast32CharactersLongForTesting!",
                ["Jwt:Issuer"] = "SailsEnergy.Tests",
                ["Jwt:Audience"] = "SailsEnergy.Tests",
                ["Jwt:AccessTokenExpirationMinutes"] = "60",
                ["Jwt:RefreshTokenExpirationDays"] = "7",
                // High rate limits for testing
                ["RateLimiting:Auth:PermitLimit"] = "10000",
                ["RateLimiting:Api:PermitLimit"] = "10000",
                ["RateLimiting:Energy:PermitLimit"] = "10000"
            });
        });

        var host = base.CreateHost(builder);

        // Initialize database after host is created
        if (!_databaseInitialized)
        {
            using var scope = host.Services.CreateScope();

            // Apply EF Core migrations
            var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            identityDb.Database.Migrate();

            var appDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDb.Database.Migrate();

            // Seed roles
            DatabaseSeeder.SeedAsync(host.Services).GetAwaiter().GetResult();

            _databaseInitialized = true;
        }

        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove existing DbContext registrations
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            // Re-register with test connection string
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(PostgresConnectionString));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(PostgresConnectionString));
        });

        // Configure Wolverine for testing - disable durability
        builder.ConfigureServices(services =>
        {
            services.Configure<WolverineOptions>(opts =>
            {
                // Use in-memory for tests to avoid DB cleanup issues
                opts.Durability.Mode = DurabilityMode.Solo;
            });
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _postgresContainer.StartAsync(),
            _rabbitMqContainer.StartAsync());
    }

    public new async Task DisposeAsync()
    {
        // Stop the host first to let Wolverine cleanup
        try
        {
            await Task.Delay(500); // Give Wolverine time to cleanup
        }
        catch { }

        await Task.WhenAll(
            _postgresContainer.StopAsync(),
            _rabbitMqContainer.StopAsync());

        await base.DisposeAsync();
    }
}
