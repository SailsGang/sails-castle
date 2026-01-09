using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Gangs.Commands.CreateGang;

namespace SailsEnergy.Application.Tests.Validators;

public class CreateGangValidatorTests
{
    private readonly CreateGangValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new CreateGangCommand("My Gang", "Description of my gang");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldPass()
    {
        var command = new CreateGangCommand("My Gang", null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithEmptyName_ShouldFail(string? name)
    {
        var command = new CreateGangCommand(name!, "Description");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldFail()
    {
        var command = new CreateGangCommand(new string('a', 101), "Description");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Gang name cannot exceed 100 characters.");
    }

    [Fact]
    public void Validate_WithTooLongDescription_ShouldFail()
    {
        var command = new CreateGangCommand("My Gang", new string('a', 501));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 500 characters.");
    }

    [Fact]
    public void Validate_WithMaxLengthName_ShouldPass()
    {
        var command = new CreateGangCommand(new string('a', 100), null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithMaxLengthDescription_ShouldPass()
    {
        var command = new CreateGangCommand("My Gang", new string('a', 500));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
