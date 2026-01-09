using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Cars.Commands.CreateCar;

namespace SailsEnergy.Application.Tests.Validators;

public class CreateCarValidatorTests
{
    private readonly CreateCarValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new CreateCarCommand("My Tesla", "Model 3", "Tesla", "ABC-123", 75.5m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithMinimalData_ShouldPass()
    {
        var command = new CreateCarCommand(null, "Model 3", "Tesla", null, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyModel_ShouldFail(string? model)
    {
        var command = new CreateCarCommand(null, model!, "Tesla", null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyManufacturer_ShouldFail(string? manufacturer)
    {
        var command = new CreateCarCommand(null, "Model 3", manufacturer!, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Manufacturer);
    }
}
