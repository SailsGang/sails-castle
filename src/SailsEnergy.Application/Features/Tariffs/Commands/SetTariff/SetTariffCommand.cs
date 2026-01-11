namespace SailsEnergy.Application.Features.Tariffs.Commands.SetTariff;

public sealed record SetTariffCommand(
    Guid GangId,
    decimal PricePerKwh,
    string Currency = "UAH");
