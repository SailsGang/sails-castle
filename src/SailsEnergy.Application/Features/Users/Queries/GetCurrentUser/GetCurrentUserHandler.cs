using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Users.Responses;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Users.Queries.GetCurrentUser;

public static class GetCurrentUserHandler
{
    public static async Task<UserProfileResponse> HandleAsync(
        GetCurrentUserQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        ILogger<GetCurrentUserQuery> logger,
        CancellationToken ct)
    {
        logger.LogInformation("GetCurrentUserHandler called for user {UserId}", currentUser.UserId);

        var userId = currentUser.UserId!.Value;

        var cached = await cache.GetEntityAsync<UserProfileResponse>(userId, ct);
        if (cached is not null)
        {
            logger.LogInformation("Returning cached profile for user {UserId}", userId);
            return cached;
        }

        var profile = await dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdentityId == userId, ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Profile not found.");

        logger.LogInformation("Profile found: {ProfileId}, DisplayName: {DisplayName}",
            profile.Id, profile.DisplayName);

        var response = new UserProfileResponse(
            profile.Id,
            currentUser.Email ?? "",
            profile.DisplayName,
            profile.FirstName,
            profile.LastName,
            profile.CreatedAt);

        await cache.SetEntityAsync(userId, response, ct);

        return response;
    }
}
