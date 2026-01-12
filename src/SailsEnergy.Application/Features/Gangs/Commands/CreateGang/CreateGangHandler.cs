using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;
using SailsEnergy.Application.Telemetry;
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
        using var activity = ActivitySources.Gangs.StartActivity("CreateGang");
        var userId = currentUser.UserId!.Value;
        activity?.SetTag("user.id", userId.ToString());

        logger.LogInformation("User {UserId} creating gang '{GangName}'", userId, command.Name);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

        try
        {
            var gang = Gang.Create(command.Name, userId, command.Description);
            dbContext.Gangs.Add(gang);

            var member = GangMember.Create(gang.Id, userId, MemberRole.Owner, userId);
            dbContext.GangMembers.Add(member);

            var period = Period.Create(gang.Id, userId);
            dbContext.Periods.Add(period);

            var defaultTariff = Tariff.Create(gang.Id, 0m, "UAH", userId);
            dbContext.Tariffs.Add(defaultTariff);

            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            await cache.RemoveAsync(CacheKeys.UserGangs(userId), ct);

            activity?.SetTag("gang.id", gang.Id.ToString());
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
