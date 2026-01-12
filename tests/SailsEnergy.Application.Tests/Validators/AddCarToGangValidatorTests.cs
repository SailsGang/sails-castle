using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Gangs.Commands.AddCarToGang;

namespace SailsEnergy.Application.Tests.Validators;

public class AddCarToGangValidatorTests
{
    private readonly AddCarToGangValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new AddCarToGangCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new AddCarToGangCommand(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
    }

    [Fact]
    public void Validate_WithEmptyCarId_ShouldFail()
    {
        var command = new AddCarToGangCommand(Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CarId);
    }

    [Fact]
    public void Validate_WithBothEmpty_ShouldFailForBoth()
    {
        var command = new AddCarToGangCommand(Guid.Empty, Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
        result.ShouldHaveValidationErrorFor(x => x.CarId);
    }
}
