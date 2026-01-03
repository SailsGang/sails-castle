namespace SailsEnergy.Domain.Common;

public class SoftDeletableEntity : AuditableEntity, ISoftDeletable
{
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public void SoftDelete(Guid userId, DateTimeOffset? timestamp = null)
    {
        IsDeleted = true;
        DeletedAt = timestamp ?? DateTimeOffset.UtcNow;
        DeletedBy = userId;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
