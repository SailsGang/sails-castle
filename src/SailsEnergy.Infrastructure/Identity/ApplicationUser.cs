using Microsoft.AspNetCore.Identity;

namespace SailsEnergy.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }

    // Link to UserProfile in Marten
    public Guid? UserProfileId { get; set; }
}
