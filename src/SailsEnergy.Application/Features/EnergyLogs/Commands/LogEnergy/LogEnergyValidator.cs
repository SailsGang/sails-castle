using FluentValidation;

namespace SailsEnergy.Application.Features.EnergyLogs.Commands.LogEnergy;

public sealed class LogEnergyValidator : AbstractValidator<LogEnergyCommand>
{
    public LogEnergyValidator()
    {
        RuleFor(x => x.GangId)
            .NotEmpty()
            .WithMessage("Gang is required.");

        RuleFor(x => x.GangCarId)
            .NotEmpty()
            .WithMessage("Car is required.");

        RuleFor(x => x.EnergyKwh)
            .GreaterThan(0)
            .WithMessage("Energy must be greater than zero.");

        RuleFor(x => x.ChargingDate)
            .NotEmpty()
            .WithMessage("Charging date is required.")
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(5))
            .WithMessage("Charging date cannot be in the future.");
    }
}
