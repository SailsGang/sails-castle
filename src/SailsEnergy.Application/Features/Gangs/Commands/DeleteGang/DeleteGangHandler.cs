using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Gangs.Commands.DeleteGang;

public static class DeleteGangHandler
{
    public static async Task HandleAsync(
        DeleteGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        ILogger<DeleteGangCommand> logger,
        CancellationToken ct)
    {
        var gang = await dbContext.Gangs.FindAsync([command.GangId], ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Gang not found.");

        if (gang.OwnerId != currentUser.UserId)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Only owner can delete the gang.");

        logger.LogInformation("User {UserId} deleting gang {GangId}", currentUser.UserId, command.GangId);

        gang.SoftDelete(currentUser.UserId!.Value);
        await dbContext.SaveChangesAsync(ct);

        await cache.InvalidateEntityAsync<GangResponse>(command.GangId, ct);
        await cache.RemoveAsync(CacheKeys.GangMembers(command.GangId), ct);
        await cache.RemoveAsync(CacheKeys.GangCars(command.GangId), ct);

        logger.LogInformation("Gang {GangId} deleted successfully", command.GangId);
    }
}
