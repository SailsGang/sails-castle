namespace SailsEnergy.Application.Features.EnergyLogs.Commands.LogEnergy;

public sealed record LogEnergyCommand(
    Guid GangId,
    Guid GangCarId,
    decimal EnergyKwh,
    DateTimeOffset ChargingDate,
    string? Notes);
