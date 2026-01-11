using FluentValidation;

namespace SailsEnergy.Application.Features.Periods.Commands.ClosePeriod;

public sealed class ClosePeriodValidator : AbstractValidator<ClosePeriodCommand>
{
    public ClosePeriodValidator()
    {
        RuleFor(x => x.GangId)
            .NotEmpty()
            .WithMessage("Gang is required.");
    }
}
