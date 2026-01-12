using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Gangs.Commands.LeaveGang;

namespace SailsEnergy.Application.Tests.Validators;

public class LeaveGangValidatorTests
{
    private readonly LeaveGangValidator _validator = new();

    [Fact]
    public void Validate_WithValidGangId_ShouldPass()
    {
        var command = new LeaveGangCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new LeaveGangCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId)
            .WithErrorMessage("Gang ID is required.");
    }
}
