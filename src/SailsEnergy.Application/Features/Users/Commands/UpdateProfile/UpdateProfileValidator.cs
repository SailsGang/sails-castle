using FluentValidation;

namespace SailsEnergy.Application.Features.Users.Commands.UpdateProfile;

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.DisplayName)
            .MaximumLength(100)
            .When(x => x.DisplayName is not null);

        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .When(x => x.LastName is not null);
    }
}
