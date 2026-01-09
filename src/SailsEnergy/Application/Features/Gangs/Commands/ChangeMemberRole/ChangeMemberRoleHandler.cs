using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Gangs.Commands.ChangeMemberRole;

public static class ChangeMemberRoleHandler
{
    public static async Task HandleAsync(
        ChangeMemberRoleCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ILogger<ChangeMemberRoleCommand> logger,
        CancellationToken ct)
    {
        var member = await dbContext.GangMembers.FindAsync([command.MemberId], ct)
                     ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Member not found.");

        if (member.GangId != command.GangId)
            throw new BusinessRuleException(ErrorCodes.NotFound, "Member not found in this gang.");

        var gang = await dbContext.Gangs.FindAsync([command.GangId], ct)
                   ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Gang not found.");

        if (gang.OwnerId != currentUser.UserId)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Only the gang owner can change roles.");

        if (member.Role == MemberRole.Owner)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Cannot change the owner's role.");

        if (!Enum.TryParse<MemberRole>(command.Role, true, out var newRole))
            throw new BusinessRuleException(ErrorCodes.ValidationFailed, "Invalid role.");

        if (newRole == MemberRole.Owner)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Use transfer ownership to change owner.");

        logger.LogInformation("User {OwnerId} changing role of member {MemberId} to {Role} in gang {GangId}", 
            currentUser.UserId, command.MemberId, command.Role, command.GangId);

        member.SetRole(newRole, currentUser.UserId!.Value);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Member {MemberId} role changed to {Role}", command.MemberId, command.Role);
    }
}
