namespace SailsEnergy.Domain.Common;

public class Entity : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity() { }

    public Guid Id { get; protected init; } = Guid.NewGuid();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
