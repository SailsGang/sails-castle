using FluentValidation;

namespace SailsEnergy.Application.Features.Gangs.Commands.DeleteGang;

public sealed class DeleteGangValidator : AbstractValidator<DeleteGangCommand>
{
    public DeleteGangValidator()
    {
        RuleFor(x => x.GangId)
            .NotEmpty().WithMessage("Gang ID is required.");
    }
}
