using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Cars.Commands.UpdateCar;

namespace SailsEnergy.Application.Tests.Validators;

public class UpdateCarValidatorTests
{
    private readonly UpdateCarValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new UpdateCarCommand(
            Guid.NewGuid(),
            "My Tesla",
            "Model 3",
            "Tesla",
            "AA1234BB",
            75.0m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyCarId_ShouldFail()
    {
        var command = new UpdateCarCommand(
            Guid.Empty,
            "My Tesla",
            "Model 3",
            "Tesla",
            "AA1234BB",
            75.0m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CarId);
    }

    [Fact]
    public void Validate_WithNullOptionalFields_ShouldPass()
    {
        var command = new UpdateCarCommand(
            Guid.NewGuid(),
            null,
            null,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNegativeBatteryCapacity_ShouldFail()
    {
        var command = new UpdateCarCommand(
            Guid.NewGuid(),
            "My Tesla",
            "Model 3",
            "Tesla",
            "AA1234BB",
            -10.0m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BatteryCapacityKwh);
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldFail()
    {
        var command = new UpdateCarCommand(
            Guid.NewGuid(),
            new string('a', 101),
            "Model 3",
            "Tesla",
            "AA1234BB",
            75.0m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
