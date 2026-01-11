using FluentValidation;

namespace SailsEnergy.Application.Features.EnergyLogs.Commands.UpdateEnergyLog;

public sealed class UpdateEnergyLogValidator : AbstractValidator<UpdateEnergyLogCommand>
{
    public UpdateEnergyLogValidator()
    {
        RuleFor(x => x.LogId)
            .NotEmpty()
            .WithMessage("Log ID is required.");

        RuleFor(x => x.EnergyKwh)
            .GreaterThan(0)
            .When(x => x.EnergyKwh.HasValue)
            .WithMessage("Energy must be greater than zero.");

        RuleFor(x => x.ChargingDate)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(5))
            .When(x => x.ChargingDate.HasValue)
            .WithMessage("Charging date cannot be in the future.");

        RuleFor(x => x)
            .Must(x => x.EnergyKwh.HasValue || x.ChargingDate.HasValue || x.Notes is not null)
            .WithMessage("At least one field must be provided to update.");
    }
}
