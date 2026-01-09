using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Domain.Tests.Entities;

public class CarTests
{
    private readonly Guid _ownerId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var car = Car.Create(_ownerId, "Model 3", "Tesla", _ownerId);

        // Assert
        car.OwnerId.Should().Be(_ownerId);
        car.Model.Should().Be("Model 3");
        car.Manufacturer.Should().Be("Tesla");
        car.Name.Should().BeEmpty();
        car.LicensePlate.Should().BeNull();
        car.BatteryCapacityKwh.Should().BeNull();
        car.Id.Should().NotBeEmpty();
        car.CreatedBy.Should().Be(_ownerId);
    }

    [Fact]
    public void SetName_WithValidName_ShouldUpdate()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Model", "Manufacturer", _ownerId);
        var updater = Guid.NewGuid();

        // Act
        car.SetName("My Tesla", updater);

        // Assert
        car.Name.Should().Be("My Tesla");
        car.UpdatedBy.Should().Be(updater);
    }

    [Fact]
    public void SetModel_WithValidModel_ShouldUpdate()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Original", "Manufacturer", _ownerId);

        // Act
        car.SetModel("New Model", Guid.NewGuid());

        // Assert
        car.Model.Should().Be("New Model");
    }

    [Fact]
    public void SetManufacturer_WithValidManufacturer_ShouldUpdate()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Model", "Original", _ownerId);

        // Act
        car.SetManufacturer("New Manufacturer", Guid.NewGuid());

        // Assert
        car.Manufacturer.Should().Be("New Manufacturer");
    }

    [Fact]
    public void SetLicensePlate_ShouldUpdate()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Model", "Manufacturer", _ownerId);

        // Act
        car.SetLicensePlate("AA1234BB", Guid.NewGuid());

        // Assert
        car.LicensePlate.Should().Be("AA1234BB");
    }

    [Fact]
    public void SetLicensePlate_ToNull_ShouldClear()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Model", "Manufacturer", _ownerId);
        car.SetLicensePlate("AA1234BB", Guid.NewGuid());

        // Act
        car.SetLicensePlate(null, Guid.NewGuid());

        // Assert
        car.LicensePlate.Should().BeNull();
    }

    [Fact]
    public void SetBatteryCapacity_ShouldUpdate()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Model", "Manufacturer", _ownerId);

        // Act
        car.SetBatteryCapacity(75.5m, Guid.NewGuid());

        // Assert
        car.BatteryCapacityKwh.Should().Be(75.5m);
    }

    [Fact]
    public void SetBatteryCapacity_ToNull_ShouldClear()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Model", "Manufacturer", _ownerId);
        car.SetBatteryCapacity(75.5m, Guid.NewGuid());

        // Act
        car.SetBatteryCapacity(null, Guid.NewGuid());

        // Assert
        car.BatteryCapacityKwh.Should().BeNull();
    }
}
