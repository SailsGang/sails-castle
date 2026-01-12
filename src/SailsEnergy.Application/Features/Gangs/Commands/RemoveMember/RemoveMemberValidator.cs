using FluentValidation;

namespace SailsEnergy.Application.Features.Gangs.Commands.RemoveMember;

public sealed class RemoveMemberValidator : AbstractValidator<RemoveMemberCommand>
{
    public RemoveMemberValidator()
    {
        RuleFor(x => x.GangId)
            .NotEmpty().WithMessage("Gang ID is required.");

        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("Member ID is required.");
    }
}
