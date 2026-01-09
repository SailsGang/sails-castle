using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Domain.Entities;

public class Car : SoftDeletableEntity
{
    public Guid OwnerId { get; private set; }
    public string? Name { get; private set; } = string.Empty;
    public string? LicensePlate { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public string Manufacturer { get; private set; } = string.Empty;
    public decimal? BatteryCapacityKwh { get; private set; }

    private Car() { } // For Marten

    public static Car Create(Guid ownerId, string model, string manufacturer, Guid createdBy)
    {
        var car = new Car
        {
            OwnerId = ownerId,
            Model =  model,
            Manufacturer = manufacturer
        };

        car.SetCreated(createdBy);

        return car;
    }

    public void SetName(string name, Guid updatedBy)
    {
        ValidationException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        Name = name;
        SetUpdated(updatedBy);
    }

    public void SetModel(string model, Guid updatedBy)
    {
        ValidationException.ThrowIfNullOrWhiteSpace(model, nameof(model));
        Model = model;
        SetUpdated(updatedBy);
    }

    public void SetManufacturer(string manufacturer, Guid updatedBy)
    {
        ValidationException.ThrowIfNullOrWhiteSpace(manufacturer, nameof(manufacturer));
        Manufacturer = manufacturer;
        SetUpdated(updatedBy);
    }

    public void SetLicensePlate(string? licensePlate, Guid updatedBy)
    {
        LicensePlate = licensePlate;
        SetUpdated(updatedBy);
    }

    public void SetBatteryCapacity(decimal? capacityKwh, Guid updatedBy)
    {
        BatteryCapacityKwh = capacityKwh;
        SetUpdated(updatedBy);
    }
}
