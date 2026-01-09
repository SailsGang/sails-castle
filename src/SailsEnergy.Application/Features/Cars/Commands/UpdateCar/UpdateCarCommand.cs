namespace SailsEnergy.Application.Features.Cars.Commands.UpdateCar;

public record UpdateCarCommand(
    Guid CarId,
    string? Name,
    string? Model,
    string? Manufacturer,
    string? LicensePlate,
    decimal? BatteryCapacityKwh);
