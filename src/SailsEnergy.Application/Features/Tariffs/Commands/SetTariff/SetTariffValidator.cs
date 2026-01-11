using FluentValidation;

namespace SailsEnergy.Application.Features.Tariffs.Commands.SetTariff;

public sealed class SetTariffValidator : AbstractValidator<SetTariffCommand>
{
    public SetTariffValidator()
    {
        RuleFor(x => x.GangId)
            .NotEmpty()
            .WithMessage("Gang is required.");

        RuleFor(x => x.PricePerKwh)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price cannot be negative.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(10)
            .WithMessage("Currency is required.");
    }
}
