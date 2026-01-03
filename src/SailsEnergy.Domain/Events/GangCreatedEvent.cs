namespace SailsEnergy.Domain.Events;

public sealed record GangCreatedEvent(Guid GangId, string Name, Guid OwnerId) : DomainEventBase;
