namespace SailsEnergy.Domain.Tests.Entities;

using SailsEnergy.Domain.Entities;

public class GangCarTests
{
    private readonly Guid _gangId = Guid.NewGuid();
    private readonly Guid _carId = Guid.NewGuid();
    private readonly Guid _memberId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var gangCar = GangCar.Create(_gangId, _carId, _memberId);

        // Assert
        gangCar.GangId.Should().Be(_gangId);
        gangCar.CarId.Should().Be(_carId);
        gangCar.IsDeleted.Should().BeFalse();
        gangCar.Id.Should().NotBeEmpty();
        gangCar.CreatedBy.Should().Be(_memberId);
        gangCar.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var gangCar = GangCar.Create(_gangId, _carId, _memberId);
        var updater = Guid.NewGuid();

        // Act
        gangCar.SoftDelete(updater);

        // Assert
        gangCar.IsDeleted.Should().BeTrue();
        gangCar.DeletedBy.Should().Be(updater);
        gangCar.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Reactivate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var gangCar = GangCar.Create(_gangId, _carId, _memberId);
        gangCar.SoftDelete(Guid.NewGuid());
        var reactivator = Guid.NewGuid();

        // Act
        gangCar.Restore();

        // Assert
        gangCar.IsDeleted.Should().BeFalse();
        gangCar.UpdatedBy.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldUseCreatedAtAsAddedAt()
    {
        // Act
        var gangCar = GangCar.Create(_gangId, _carId, _memberId);

        // Assert
        // CreatedAt serves as the "added at" timestamp
        gangCar.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }
}
