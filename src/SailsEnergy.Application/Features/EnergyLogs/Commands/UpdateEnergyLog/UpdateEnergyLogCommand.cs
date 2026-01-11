namespace SailsEnergy.Application.Features.EnergyLogs.Commands.UpdateEnergyLog;

public sealed record UpdateEnergyLogCommand(
    Guid LogId,
    decimal? EnergyKwh,
    DateTimeOffset? ChargingDate,
    string? Notes);
