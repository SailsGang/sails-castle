using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Gangs.Commands.RemoveMember;

namespace SailsEnergy.Application.Tests.Validators;

public class RemoveMemberValidatorTests
{
    private readonly RemoveMemberValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new RemoveMemberCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new RemoveMemberCommand(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId)
            .WithErrorMessage("Gang ID is required.");
    }

    [Fact]
    public void Validate_WithEmptyMemberId_ShouldFail()
    {
        var command = new RemoveMemberCommand(Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MemberId)
            .WithErrorMessage("Member ID is required.");
    }

    [Fact]
    public void Validate_WithBothEmpty_ShouldFailForBoth()
    {
        var command = new RemoveMemberCommand(Guid.Empty, Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
        result.ShouldHaveValidationErrorFor(x => x.MemberId);
    }
}
