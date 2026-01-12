using FluentValidation;

namespace SailsEnergy.Application.Features.Gangs.Commands.LeaveGang;

public sealed class LeaveGangValidator : AbstractValidator<LeaveGangCommand>
{
    public LeaveGangValidator()
    {
        RuleFor(x => x.GangId)
            .NotEmpty().WithMessage("Gang ID is required.");
    }
}
