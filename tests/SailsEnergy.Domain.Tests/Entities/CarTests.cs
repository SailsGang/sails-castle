namespace SailsEnergy.Domain.Tests.Entities;

using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;

public class CarTests
{
    private readonly Guid _ownerId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var car = Car.Create(_ownerId, "My Tesla", "Model 3", "Tesla", _ownerId);

        // Assert
        car.OwnerId.Should().Be(_ownerId);
        car.Name.Should().Be("My Tesla");
        car.Model.Should().Be("Model 3");
        car.Manufacturer.Should().Be("Tesla");
        car.LicensePlate.Should().BeNull();
        car.BatteryCapacityKwh.Should().BeNull();
        car.Id.Should().NotBeEmpty();
        car.CreatedBy.Should().Be(_ownerId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrow(string? name)
    {
        // Act & Assert
        var act = () => Car.Create(_ownerId, name!, "Model", "Manufacturer", _ownerId);
        act.Should().Throw<ValidationException>()
           .Where(e => e.PropertyName == "name");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidModel_ShouldThrow(string? model)
    {
        // Act & Assert
        var act = () => Car.Create(_ownerId, "Name", model!, "Manufacturer", _ownerId);
        act.Should().Throw<ValidationException>()
           .Where(e => e.PropertyName == "model");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidManufacturer_ShouldThrow(string? manufacturer)
    {
        // Act & Assert
        var act = () => Car.Create(_ownerId, "Name", "Model", manufacturer!, _ownerId);
        act.Should().Throw<ValidationException>()
           .Where(e => e.PropertyName == "manufacturer");
    }

    [Fact]
    public void SetName_WithValidName_ShouldUpdate()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Original", "Model", "Manufacturer", _ownerId);
        var updater = Guid.NewGuid();

        // Act
        car.SetName("Updated", updater);

        // Assert
        car.Name.Should().Be("Updated");
        car.UpdatedBy.Should().Be(updater);
    }

    [Fact]
    public void SetModel_WithValidModel_ShouldUpdate()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Name", "Original", "Manufacturer", _ownerId);

        // Act
        car.SetModel("New Model", Guid.NewGuid());

        // Assert
        car.Model.Should().Be("New Model");
    }

    [Fact]
    public void SetManufacturer_WithValidManufacturer_ShouldUpdate()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Name", "Model", "Original", _ownerId);

        // Act
        car.SetManufacturer("New Manufacturer", Guid.NewGuid());

        // Assert
        car.Manufacturer.Should().Be("New Manufacturer");
    }

    [Fact]
    public void SetLicensePlate_ShouldUpdate()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Name", "Model", "Manufacturer", _ownerId);

        // Act
        car.SetLicensePlate("AA1234BB", Guid.NewGuid());

        // Assert
        car.LicensePlate.Should().Be("AA1234BB");
    }

    [Fact]
    public void SetLicensePlate_ToNull_ShouldClear()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Name", "Model", "Manufacturer", _ownerId);
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
        var car = Car.Create(_ownerId, "Name", "Model", "Manufacturer", _ownerId);

        // Act
        car.SetBatteryCapacity(75.5m, Guid.NewGuid());

        // Assert
        car.BatteryCapacityKwh.Should().Be(75.5m);
    }

    [Fact]
    public void SetBatteryCapacity_ToNull_ShouldClear()
    {
        // Arrange
        var car = Car.Create(_ownerId, "Name", "Model", "Manufacturer", _ownerId);
        car.SetBatteryCapacity(75.5m, Guid.NewGuid());

        // Act
        car.SetBatteryCapacity(null, Guid.NewGuid());

        // Assert
        car.BatteryCapacityKwh.Should().BeNull();
    }
}
