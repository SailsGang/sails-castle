namespace SailsEnergy.Domain.Tests.Entities;

using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Events;
using SailsEnergy.Domain.Exceptions;

public class GangTests
{
    private readonly Guid _ownerId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var gang = Gang.Create("Test Gang", _ownerId, "Description");

        // Assert
        gang.Name.Should().Be("Test Gang");
        gang.Description.Should().Be("Description");
        gang.OwnerId.Should().Be(_ownerId);
        gang.Id.Should().NotBeEmpty();
        gang.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        gang.CreatedBy.Should().Be(_ownerId);
    }

    [Fact]
    public void Create_WithoutDescription_ShouldSucceed()
    {
        // Act
        var gang = Gang.Create("Test Gang", _ownerId);

        // Assert
        gang.Name.Should().Be("Test Gang");
        gang.Description.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrow(string? name)
    {
        // Act & Assert
        var act = () => Gang.Create(name!, _ownerId);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_ShouldRaiseGangCreatedEvent()
    {
        // Act
        var gang = Gang.Create("Test Gang", _ownerId);

        // Assert
        gang.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<GangCreatedEvent>()
            .Which.Should().Match<GangCreatedEvent>(e =>
                e.GangId == gang.Id &&
                e.Name == "Test Gang" &&
                e.OwnerId == _ownerId);
    }

    [Fact]
    public void SetName_WithValidName_ShouldUpdate()
    {
        // Arrange
        var gang = Gang.Create("Original", _ownerId);
        var updater = Guid.NewGuid();

        // Act
        gang.SetName("Updated", updater);

        // Assert
        gang.Name.Should().Be("Updated");
        gang.UpdatedBy.Should().Be(updater);
        gang.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetName_WithInvalidName_ShouldThrow(string? name)
    {
        // Arrange
        var gang = Gang.Create("Original", _ownerId);

        // Act & Assert
        var act = () => gang.SetName(name!, Guid.NewGuid());
        act.Should().Throw<ValidationException>()
           .Where(e => e.PropertyName == "name");
    }

    [Fact]
    public void SetDescription_ShouldUpdate()
    {
        // Arrange
        var gang = Gang.Create("Name", _ownerId);
        var updater = Guid.NewGuid();

        // Act
        gang.SetDescription("New Description", updater);

        // Assert
        gang.Description.Should().Be("New Description");
        gang.UpdatedBy.Should().Be(updater);
    }

    [Fact]
    public void SetDescription_ToNull_ShouldClear()
    {
        // Arrange
        var gang = Gang.Create("Name", _ownerId, "Initial");

        // Act
        gang.SetDescription(null, Guid.NewGuid());

        // Assert
        gang.Description.Should().BeNull();
    }

    [Fact]
    public void TransferOwnership_ShouldChangeOwner()
    {
        // Arrange
        var gang = Gang.Create("Gang", _ownerId);
        var newOwner = Guid.NewGuid();

        // Act
        gang.TransferOwnership(newOwner, _ownerId);

        // Assert
        gang.OwnerId.Should().Be(newOwner);
        gang.UpdatedBy.Should().Be(_ownerId);
    }
}
