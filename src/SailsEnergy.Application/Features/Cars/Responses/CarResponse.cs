namespace SailsEnergy.Application.Features.Cars.Responses;

public sealed record CarResponse(
    Guid Id,
    string? Name,
    string Model,
    string Manufacturer,
    string? LicensePlate,
    decimal? BatteryCapacityKwh,
    DateTimeOffset CreatedAt);
