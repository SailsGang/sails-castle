using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SailsEnergy.Application.Notifications;

namespace SailsEnergy.Api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task JoinGang(Guid gangId) => await Groups.AddToGroupAsync(Context.ConnectionId, NotificationExtensions.GangGroup(gangId));

    public async Task LeaveGang(Guid gangId) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, NotificationExtensions.GangGroup(gangId));

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId)) await Groups.AddToGroupAsync(Context.ConnectionId, NotificationExtensions.UserGroup(Guid.Parse(userId)));
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId)) await Groups.RemoveFromGroupAsync(Context.ConnectionId, NotificationExtensions.UserGroup(Guid.Parse(userId)));
        await base.OnDisconnectedAsync(exception);
    }
}
