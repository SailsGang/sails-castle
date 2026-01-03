namespace SailsEnergy.Domain.Events;

public sealed record PeriodClosedEvent(Guid GangId, Guid PeriodId, Guid ClosedByUserId) : DomainEventBase;
