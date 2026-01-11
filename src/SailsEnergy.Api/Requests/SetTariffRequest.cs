namespace SailsEnergy.Api.Requests;

public sealed record SetTariffRequest(decimal PricePerKwh, string Currency = "UAH");
