using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.EnergyLogs.Commands.LogEnergy;

namespace SailsEnergy.Application.Tests.Validators;

public class LogEnergyValidatorTests
{
    private readonly LogEnergyValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new LogEnergyCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50.5m,
            DateTimeOffset.UtcNow.AddHours(-1),
            "Test notes");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new LogEnergyCommand(
            Guid.Empty,
            Guid.NewGuid(),
            50.5m,
            DateTimeOffset.UtcNow,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
    }

    [Fact]
    public void Validate_WithEmptyGangCarId_ShouldFail()
    {
        var command = new LogEnergyCommand(
            Guid.NewGuid(),
            Guid.Empty,
            50.5m,
            DateTimeOffset.UtcNow,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangCarId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithInvalidEnergyKwh_ShouldFail(decimal energyKwh)
    {
        var command = new LogEnergyCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            energyKwh,
            DateTimeOffset.UtcNow,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EnergyKwh);
    }

    [Fact]
    public void Validate_WithFutureChargingDate_ShouldFail()
    {
        var command = new LogEnergyCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50.5m,
            DateTimeOffset.UtcNow.AddHours(1),
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ChargingDate);
    }

    [Fact]
    public void Validate_WithNullNotes_ShouldPass()
    {
        var command = new LogEnergyCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50.5m,
            DateTimeOffset.UtcNow.AddMinutes(-30),
            null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
