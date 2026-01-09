using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Users.Responses;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Users.Commands.UpdateProfile;

public static class UpdateProfileHandler
{
    public static async Task HandleAsync(
        UpdateProfileCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        CancellationToken ct)
    {
        var profile = await dbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.IdentityId == currentUser.UserId, ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Profile not found.");

        if (command.DisplayName is not null)
            profile.SetDisplayName(command.DisplayName, currentUser.UserId!.Value);

        if (command.FirstName is not null || command.LastName is not null)
            profile.SetName(command.FirstName, command.LastName, currentUser.UserId!.Value);

        await dbContext.SaveChangesAsync(ct);
        await cache.InvalidateEntityAsync<UserProfileResponse>(currentUser.UserId!.Value, ct);
    }
}
