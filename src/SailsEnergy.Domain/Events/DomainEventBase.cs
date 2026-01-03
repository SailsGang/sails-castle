using SailsEnergy.Domain.Common;

namespace SailsEnergy.Domain.Events;

public abstract record DomainEventBase : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
