using FluentValidation;

namespace SailsEnergy.Application.Features.EnergyLogs.Commands.DeleteEnergyLog;

public sealed class DeleteEnergyLogValidator : AbstractValidator<DeleteEnergyLogCommand>
{
    public DeleteEnergyLogValidator()
    {
        RuleFor(x => x.LogId)
            .NotEmpty()
            .WithMessage("Log ID is required.");
    }
}
