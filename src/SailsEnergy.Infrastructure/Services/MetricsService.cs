using System.Diagnostics.Metrics;

namespace SailsEnergy.Infrastructure.Services;

public class MetricsService
{
    public static readonly Meter Meter = new("SailsEnergy.Api", "1.0.0");

    public static readonly Counter<int> LoginAttempts = Meter.CreateCounter<int>("auth.login.attempts");
    public static readonly Counter<int> LoginSuccesses = Meter.CreateCounter<int>("auth.login.successes");
    public static readonly Counter<int> LoginFailures = Meter.CreateCounter<int>("auth.login.failures");
    public static readonly Counter<int> Registrations = Meter.CreateCounter<int>("auth.registrations");
}
