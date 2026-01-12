using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Gangs.Commands.RemoveCarFromGang;

namespace SailsEnergy.Application.Tests.Validators;

public class RemoveCarFromGangValidatorTests
{
    private readonly RemoveCarFromGangValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new RemoveCarFromGangCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new RemoveCarFromGangCommand(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId)
            .WithErrorMessage("Gang ID is required.");
    }

    [Fact]
    public void Validate_WithEmptyCarId_ShouldFail()
    {
        var command = new RemoveCarFromGangCommand(Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CarId)
            .WithErrorMessage("Car ID is required.");
    }

    [Fact]
    public void Validate_WithBothEmpty_ShouldFailForBoth()
    {
        var command = new RemoveCarFromGangCommand(Guid.Empty, Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
        result.ShouldHaveValidationErrorFor(x => x.CarId);
    }
}
