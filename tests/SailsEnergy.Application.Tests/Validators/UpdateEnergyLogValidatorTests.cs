using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.EnergyLogs.Commands.UpdateEnergyLog;

namespace SailsEnergy.Application.Tests.Validators;

public class UpdateEnergyLogValidatorTests
{
    private readonly UpdateEnergyLogValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new UpdateEnergyLogCommand(
            Guid.NewGuid(),
            75.5m,
            DateTimeOffset.UtcNow.AddHours(-1),
            "Updated notes");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyLogId_ShouldFail()
    {
        var command = new UpdateEnergyLogCommand(
            Guid.Empty,
            75.5m,
            null,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LogId);
    }

    [Fact]
    public void Validate_WithOnlyEnergyKwh_ShouldPass()
    {
        var command = new UpdateEnergyLogCommand(
            Guid.NewGuid(),
            75.5m,
            null,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithOnlyNotes_ShouldPass()
    {
        var command = new UpdateEnergyLogCommand(
            Guid.NewGuid(),
            null,
            null,
            "Updated notes");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNoUpdates_ShouldFail()
    {
        var command = new UpdateEnergyLogCommand(
            Guid.NewGuid(),
            null,
            null,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveAnyValidationError();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidEnergyKwh_ShouldFail(decimal energyKwh)
    {
        var command = new UpdateEnergyLogCommand(
            Guid.NewGuid(),
            energyKwh,
            null,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EnergyKwh);
    }

    [Fact]
    public void Validate_WithFutureChargingDate_ShouldFail()
    {
        var command = new UpdateEnergyLogCommand(
            Guid.NewGuid(),
            null,
            DateTimeOffset.UtcNow.AddHours(1),
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ChargingDate);
    }
}
