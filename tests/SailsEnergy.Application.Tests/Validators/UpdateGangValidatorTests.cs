using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Gangs.Commands.UpdateGang;

namespace SailsEnergy.Application.Tests.Validators;

public class UpdateGangValidatorTests
{
    private readonly UpdateGangValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new UpdateGangCommand(Guid.NewGuid(), "Updated Name", "Updated Description");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new UpdateGangCommand(Guid.Empty, "Updated Name", "Updated Description");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
    }

    [Fact]
    public void Validate_WithNullNameAndDescription_ShouldPass()
    {
        var command = new UpdateGangCommand(Guid.NewGuid(), null, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldFail()
    {
        var command = new UpdateGangCommand(Guid.NewGuid(), new string('a', 101), null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithTooLongDescription_ShouldFail()
    {
        var command = new UpdateGangCommand(Guid.NewGuid(), null, new string('a', 501));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithMaxLengthName_ShouldPass()
    {
        var command = new UpdateGangCommand(Guid.NewGuid(), new string('a', 100), null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
