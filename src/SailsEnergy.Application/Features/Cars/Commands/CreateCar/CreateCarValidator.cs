using FluentValidation;

namespace SailsEnergy.Application.Features.Cars.Commands.CreateCar;

public sealed class CreateCarValidator : AbstractValidator<CreateCarCommand>
{
    public CreateCarValidator()
    {
        RuleFor(x => x.Name).MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Manufacturer).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LicensePlate).MaximumLength(20);
        RuleFor(x => x.BatteryCapacityKwh).GreaterThan(0).When(x => x.BatteryCapacityKwh.HasValue);
    }
}
