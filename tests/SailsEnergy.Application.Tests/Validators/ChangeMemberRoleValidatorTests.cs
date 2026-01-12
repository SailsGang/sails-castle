using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Gangs.Commands.ChangeMemberRole;

namespace SailsEnergy.Application.Tests.Validators;

public class ChangeMemberRoleValidatorTests
{
    private readonly ChangeMemberRoleValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new ChangeMemberRoleCommand(Guid.NewGuid(), Guid.NewGuid(), "Admin");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new ChangeMemberRoleCommand(Guid.Empty, Guid.NewGuid(), "Admin");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
    }

    [Fact]
    public void Validate_WithEmptyMemberId_ShouldFail()
    {
        var command = new ChangeMemberRoleCommand(Guid.NewGuid(), Guid.Empty, "Admin");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MemberId);
    }

    [Theory]
    [InlineData("Member")]
    [InlineData("Admin")]
    public void Validate_WithValidRoles_ShouldPass(string role)
    {
        var command = new ChangeMemberRoleCommand(Guid.NewGuid(), Guid.NewGuid(), role);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithEmptyRole_ShouldFail(string? role)
    {
        var command = new ChangeMemberRoleCommand(Guid.NewGuid(), Guid.NewGuid(), role!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Role);
    }
}
