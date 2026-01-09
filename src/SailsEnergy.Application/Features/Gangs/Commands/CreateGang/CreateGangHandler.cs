using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Gangs.Commands.CreateGang;

public static class CreateGangHandler
{
    public static async Task<CreateGangResponse> HandleAsync(
        CreateGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        ILogger<CreateGangCommand> logger,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;

        logger.LogInformation("User {UserId} creating gang '{GangName}'", userId, command.Name);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

        try
        {
            var gang = Gang.Create(command.Name, userId, command.Description);
            dbContext.Gangs.Add(gang);

            var member = GangMember.Create(gang.Id, userId, MemberRole.Owner, userId);
            dbContext.GangMembers.Add(member);

            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            await cache.RemoveAsync(CacheKeys.UserGangs(userId), ct);

            logger.LogInformation("Gang {GangId} created successfully", gang.Id);

            return new CreateGangResponse(gang.Id);
        }
        catch
        {
            await transaction.RollbackAsync(ct);

            throw;
        }
    }
}
