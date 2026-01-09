using FluentValidation;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Gangs.Commands.ChangeMemberRole;

public sealed class ChangeMemberRoleValidator : AbstractValidator<ChangeMemberRoleCommand>
{
    public ChangeMemberRoleValidator()
    {
        RuleFor(x => x.GangId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => r is nameof(MemberRole.Admin) or nameof(MemberRole.Member))
            .WithMessage($"Role must be '{nameof(MemberRole.Admin)}' or '{nameof(MemberRole.Member)}'.");
    }
}
