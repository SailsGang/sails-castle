using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Events;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Domain.Entities;

public class Gang : SoftDeletableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }

    private Gang() { } // For Marten

    public static Gang Create(string name, Guid ownerId, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var gang = new Gang
        {
            Name = name,
            Description = description,
            OwnerId = ownerId
        };

        gang.SetCreated(ownerId);

        gang.AddDomainEvent(new GangCreatedEvent(gang.Id, name, ownerId));

        return gang;
    }

    public void SetName(string name, Guid updatedBy)
    {
        ValidationException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        Name = name;
        SetUpdated(updatedBy);
    }

    public void SetDescription(string? description, Guid updatedBy)
    {
        Description = description;
        SetUpdated(updatedBy);
    }

    public void TransferOwnership(Guid newOwnerId, Guid transferredBy)
    {
        OwnerId = newOwnerId;
        SetUpdated(transferredBy);
    }
}
