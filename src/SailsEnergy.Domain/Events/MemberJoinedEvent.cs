namespace SailsEnergy.Domain.Events;

public sealed record MemberJoinedEvent(Guid GangId, Guid UserId, Guid MemberId) : DomainEventBase;
