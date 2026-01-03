using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Domain.Entities;

public class UserProfile : SoftDeletableEntity
{
    public Guid IdentityId { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public Uri? AvatarUrl { get; private set; }

    private UserProfile() { } // For Marten

    public static UserProfile Create(Guid identityId, string displayName, Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        var profile = new UserProfile
        {
            IdentityId = identityId,
            DisplayName = displayName
        };

        profile.SetCreated(createdBy);

        return profile;
    }


    public void SetDisplayName(string displayName, Guid updatedBy)
    {
        ValidationException.ThrowIfNullOrWhiteSpace(displayName, nameof(displayName));

        DisplayName = displayName;
        SetUpdated(updatedBy);
    }

    public void SetName(string? firstName, string? lastName, Guid updatedBy)
    {
        FirstName = firstName;
        LastName = lastName;
        SetUpdated(updatedBy);
    }

    public void SetAvatar(Uri? avatarUrl, Guid updatedBy)
    {
        AvatarUrl = avatarUrl;
        SetUpdated(updatedBy);
    }
}
