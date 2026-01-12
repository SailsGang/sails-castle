using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Cars.Commands.DeleteCar;

namespace SailsEnergy.Application.Tests.Validators;

public class DeleteCarValidatorTests
{
    private readonly DeleteCarValidator _validator = new();

    [Fact]
    public void Validate_WithValidCarId_ShouldPass()
    {
        var command = new DeleteCarCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyCarId_ShouldFail()
    {
        var command = new DeleteCarCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CarId)
            .WithErrorMessage("Car ID is required.");
    }
}
