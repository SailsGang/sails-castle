namespace SailsEnergy.Application.Features.Cars.Responses;

public record CarResponse(
    Guid Id,
    string? Name,
    string Model,
    string Manufacturer,
    string? LicensePlate,
    decimal? BatteryCapacityKwh,
    DateTimeOffset CreatedAt);
