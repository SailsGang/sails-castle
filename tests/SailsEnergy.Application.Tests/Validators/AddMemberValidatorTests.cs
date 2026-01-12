using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Gangs.Commands.AddMember;

namespace SailsEnergy.Application.Tests.Validators;

public class AddMemberValidatorTests
{
    private readonly AddMemberValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new AddMemberCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new AddMemberCommand(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
    }

    [Fact]
    public void Validate_WithEmptyUserId_ShouldFail()
    {
        var command = new AddMemberCommand(Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WithBothEmpty_ShouldFailForBoth()
    {
        var command = new AddMemberCommand(Guid.Empty, Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
