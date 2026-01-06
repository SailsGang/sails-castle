namespace SailsEnergy.Domain.Common;

public class AuditableEntity : Entity, IAuditable
{
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    protected AuditableEntity() { }

    protected void SetCreated(Guid userId, DateTimeOffset? timestamp = null)
    {
        CreatedAt = timestamp ?? DateTimeOffset.UtcNow;
        CreatedBy = userId;
    }

    public void SetUpdated(Guid userId, DateTimeOffset? timestamp = null)
    {
        UpdatedAt = timestamp ?? DateTimeOffset.UtcNow;
        UpdatedBy = userId;
    }
}
