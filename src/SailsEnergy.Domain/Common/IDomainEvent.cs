namespace SailsEnergy.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
