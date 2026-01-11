using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Periods.Commands.ClosePeriod;

namespace SailsEnergy.Application.Tests.Validators;

public class ClosePeriodValidatorTests
{
    private readonly ClosePeriodValidator _validator = new();

    [Fact]
    public void Validate_WithValidGangId_ShouldPass()
    {
        var command = new ClosePeriodCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new ClosePeriodCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
    }
}
