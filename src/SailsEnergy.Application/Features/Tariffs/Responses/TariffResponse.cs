namespace SailsEnergy.Application.Features.Tariffs.Responses;

public sealed record TariffResponse(
    Guid Id,
    Guid GangId,
    decimal PricePerKwh,
    string Currency,
    DateTimeOffset EffectiveFrom,
    Guid SetByUserId);

public sealed record TariffHistoryEntry(
    decimal PricePerKwh,
    string Currency,
    DateTimeOffset EffectiveFrom,
    Guid SetByUserId,
    string SetByUserName);
