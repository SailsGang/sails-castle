namespace SailsEnergy.Domain.Events;

public sealed record TariffChangedEvent(
    Guid GangId,
    Guid TariffId,
    decimal PricePerKwh,
    string Currency) : DomainEventBase;
