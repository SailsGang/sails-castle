using System.ComponentModel.DataAnnotations;

namespace SailsEnergy.Application.Features.Cars.Commands.CreateCar;

public record CreateCarCommand(
    string? Name,
    [Required] string Model,
    [Required] string Manufacturer,
    string? LicensePlate,
    decimal? BatteryCapacityKwh);
