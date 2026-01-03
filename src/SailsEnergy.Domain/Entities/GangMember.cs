using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Events;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Domain.Entities;

public class GangMember : AuditableEntity
{
    public Guid GangId { get; private set; }
    public Guid UserId { get; private set; }
    public MemberRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private GangMember() { }

    public static GangMember Create(Guid gangId, Guid userId, MemberRole role, Guid createdBy)
    {
        var member = new GangMember
        {
            GangId = gangId,
            UserId = userId,
            Role = role,
            IsActive = true
        };

        member.SetCreated(createdBy);
        member.AddDomainEvent(new MemberJoinedEvent(gangId, userId, member.Id));

        return member;
    }

    public void SetRole(MemberRole role, Guid updatedBy)
    {
        Role = role;
        SetUpdated(updatedBy);
    }

    public void Deactivate(Guid updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    public void Reactivate(Guid updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }
}
