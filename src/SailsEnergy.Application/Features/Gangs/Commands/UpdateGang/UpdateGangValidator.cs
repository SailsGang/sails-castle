using FluentValidation;

namespace SailsEnergy.Application.Features.Gangs.Commands.UpdateGang;

public sealed class UpdateGangValidator : AbstractValidator<UpdateGangCommand>
{
    public UpdateGangValidator()
    {
        RuleFor(x => x.GangId).NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);
    }
}
