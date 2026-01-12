using FluentValidation;

namespace SailsEnergy.Application.Features.Gangs.Commands.RemoveCarFromGang;

public sealed class RemoveCarFromGangValidator : AbstractValidator<RemoveCarFromGangCommand>
{
    public RemoveCarFromGangValidator()
    {
        RuleFor(x => x.GangId)
            .NotEmpty().WithMessage("Gang ID is required.");

        RuleFor(x => x.CarId)
            .NotEmpty().WithMessage("Car ID is required.");
    }
}
