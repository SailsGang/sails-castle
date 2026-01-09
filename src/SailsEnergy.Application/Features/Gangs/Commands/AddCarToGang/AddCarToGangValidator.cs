using FluentValidation;

namespace SailsEnergy.Application.Features.Gangs.Commands.AddCarToGang;

public sealed class AddCarToGangValidator : AbstractValidator<AddCarToGangCommand>
{
    public AddCarToGangValidator()
    {
        RuleFor(x => x.GangId).NotEmpty();
        RuleFor(x => x.CarId).NotEmpty();
    }
}
