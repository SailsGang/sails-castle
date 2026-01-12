using FluentValidation;

namespace SailsEnergy.Application.Features.Cars.Commands.DeleteCar;

public sealed class DeleteCarValidator : AbstractValidator<DeleteCarCommand>
{
    public DeleteCarValidator()
    {
        RuleFor(x => x.CarId)
            .NotEmpty().WithMessage("Car ID is required.");
    }
}
