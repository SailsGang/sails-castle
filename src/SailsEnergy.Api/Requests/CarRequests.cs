namespace SailsEnergy.Api.Requests;

public record UpdateCarRequest(
    string? Name,
    string? Model,
    string? Manufacturer,
    string? LicensePlate,
    decimal? BatteryCapacityKwh);
