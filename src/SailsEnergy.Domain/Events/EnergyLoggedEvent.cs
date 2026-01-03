namespace SailsEnergy.Domain.Events;

public sealed record EnergyLoggedEvent(
    Guid GangId,
    Guid PeriodId,
    Guid EnergyLogId,
    decimal EnergyKwh) : DomainEventBase;
