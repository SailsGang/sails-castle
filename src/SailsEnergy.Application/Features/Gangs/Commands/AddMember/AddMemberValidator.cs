using FluentValidation;

namespace SailsEnergy.Application.Features.Gangs.Commands.AddMember;

public sealed class AddMemberValidator : AbstractValidator<AddMemberCommand>
{
    public AddMemberValidator()
    {
        RuleFor(x => x.GangId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
