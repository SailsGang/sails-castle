namespace SailsEnergy.Domain.Tests.Entities;

using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Events;
using SailsEnergy.Domain.Exceptions;

public class EnergyLogTests
{
    private readonly Guid _gangId = Guid.NewGuid();
    private readonly Guid _gangCarId = Guid.NewGuid();
    private readonly Guid _periodId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _tariffId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var chargingDate = DateTimeOffset.UtcNow.AddHours(-1);

        // Act
        var log = EnergyLog.Create(
            _gangId, _gangCarId, _periodId, _userId,
            energyKwh: 50.5m,
            chargingDate: chargingDate,
            _tariffId, notes: "Test note", createdBy: _userId);

        // Assert
        log.GangId.Should().Be(_gangId);
        log.GangCarId.Should().Be(_gangCarId);
        log.PeriodId.Should().Be(_periodId);
        log.LoggedByUserId.Should().Be(_userId);
        log.EnergyKwh.Should().Be(50.5m);
        log.ChargingDate.Should().Be(chargingDate);
        log.AppliedTariffId.Should().Be(_tariffId);
        log.Notes.Should().Be("Test note");
        log.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldRaiseEnergyLoggedEvent()
    {
        // Act
        var log = EnergyLog.Create(
            _gangId, _gangCarId, _periodId, _userId,
            50.5m, DateTimeOffset.UtcNow, _tariffId, null, _userId);

        // Assert
        log.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<EnergyLoggedEvent>()
            .Which.Should().Match<EnergyLoggedEvent>(e =>
                e.GangId == _gangId &&
                e.PeriodId == _periodId &&
                e.EnergyLogId == log.Id &&
                e.EnergyKwh == 50.5m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.5)]
    public void Create_WithInvalidEnergy_ShouldThrow(decimal energyKwh)
    {
        // Act & Assert
        var act = () => EnergyLog.Create(
            _gangId, _gangCarId, _periodId, _userId,
            energyKwh, DateTimeOffset.UtcNow, _tariffId, null, _userId);

        act.Should().Throw<ValidationException>()
           .Where(e => e.PropertyName == "energyKwh");
    }

    [Fact]
    public void SetEnergyKwh_WithinEditWindow_ShouldUpdate()
    {
        // Arrange
        var log = EnergyLog.Create(
            _gangId, _gangCarId, _periodId, _userId,
            50m, DateTimeOffset.UtcNow, _tariffId, null, _userId);

        // Act
        log.SetEnergyKwh(75m, _userId);

        // Assert
        log.EnergyKwh.Should().Be(75m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void SetEnergyKwh_WithInvalidValue_ShouldThrow(decimal energyKwh)
    {
        // Arrange
        var log = EnergyLog.Create(
            _gangId, _gangCarId, _periodId, _userId,
            50m, DateTimeOffset.UtcNow, _tariffId, null, _userId);

        // Act & Assert
        var act = () => log.SetEnergyKwh(energyKwh, _userId);
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void SetNotes_WithinEditWindow_ShouldUpdate()
    {
        // Arrange
        var log = EnergyLog.Create(
            _gangId, _gangCarId, _periodId, _userId,
            50m, DateTimeOffset.UtcNow, _tariffId, null, _userId);

        // Act
        log.SetNotes("Updated note", _userId);

        // Assert
        log.Notes.Should().Be("Updated note");
    }

    [Fact]
    public void SetChargingDate_WithinEditWindow_ShouldUpdate()
    {
        // Arrange
        var log = EnergyLog.Create(
            _gangId, _gangCarId, _periodId, _userId,
            50m, DateTimeOffset.UtcNow, _tariffId, null, _userId);
        var newDate = DateTimeOffset.UtcNow.AddDays(-1);

        // Act
        log.SetChargingDate(newDate, _userId);

        // Assert
        log.ChargingDate.Should().Be(newDate);
    }
}
