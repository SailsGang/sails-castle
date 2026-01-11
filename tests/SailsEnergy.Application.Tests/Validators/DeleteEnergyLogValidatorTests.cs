using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.EnergyLogs.Commands.DeleteEnergyLog;

namespace SailsEnergy.Application.Tests.Validators;

public class DeleteEnergyLogValidatorTests
{
    private readonly DeleteEnergyLogValidator _validator = new();

    [Fact]
    public void Validate_WithValidLogId_ShouldPass()
    {
        var command = new DeleteEnergyLogCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyLogId_ShouldFail()
    {
        var command = new DeleteEnergyLogCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LogId);
    }
}
