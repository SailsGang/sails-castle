using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Gangs.Commands.DeleteGang;

namespace SailsEnergy.Application.Tests.Validators;

public class DeleteGangValidatorTests
{
    private readonly DeleteGangValidator _validator = new();

    [Fact]
    public void Validate_WithValidGangId_ShouldPass()
    {
        var command = new DeleteGangCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new DeleteGangCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId)
            .WithErrorMessage("Gang ID is required.");
    }
}
