using FluentValidation;

namespace SailsEnergy.Application.Features.Gangs.Commands.CreateGang;

public sealed class CreateGangValidator : AbstractValidator<CreateGangCommand>
{
    public CreateGangValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Gang name is required.")
            .MaximumLength(100).WithMessage("Gang name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
