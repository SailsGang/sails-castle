using FluentValidation;

namespace SailsEnergy.Application.Features.Cars.Commands.UpdateCar;

public sealed class UpdateCarValidator : AbstractValidator<UpdateCarCommand>
{
    public UpdateCarValidator()
    {
        RuleFor(x => x.CarId).NotEmpty();
        RuleFor(x => x.Name).MaximumLength(100).When(x => x.Name != null);
        RuleFor(x => x.Model).MaximumLength(100).When(x => x.Model != null);
        RuleFor(x => x.Manufacturer).MaximumLength(100).When(x => x.Manufacturer != null);
        RuleFor(x => x.LicensePlate).MaximumLength(20).When(x => x.LicensePlate != null);
        RuleFor(x => x.BatteryCapacityKwh).GreaterThan(0).When(x => x.BatteryCapacityKwh.HasValue);
    }
}
