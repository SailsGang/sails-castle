namespace SailsEnergy.Application.Features.EnergyLogs.Responses;

public sealed record EnergyLogResponse(
    Guid Id,
    Guid GangId,
    Guid GangCarId,
    string CarName,
    Guid PeriodId,
    Guid LoggedByUserId,
    string LoggedByUserName,
    decimal EnergyKwh,
    decimal CostUah,
    DateTimeOffset ChargingDate,
    string? Notes,
    bool CanEdit,
    DateTimeOffset CreatedAt);

public sealed record LogEnergyResponse(Guid LogId, decimal CostUah);
