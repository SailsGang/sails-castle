using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Notifications;

namespace SailsEnergy.Infrastructure.Services;

public class SignalRNotificationService<THub> : IRealtimeNotificationService
    where THub : Hub
{
    private readonly IHubContext<THub> _hubContext;
    private readonly ILogger<SignalRNotificationService<THub>> _logger;

    public SignalRNotificationService(
        IHubContext<THub> hubContext,
        ILogger<SignalRNotificationService<THub>> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendToGroupAsync<T>(
        string groupName,
        string eventName,
        T payload,
        CancellationToken ct = default) where T : class
    {
        _logger.LogDebug("Sending {Event} to group {Group}", eventName, groupName);
        await _hubContext.Clients.Group(groupName).SendAsync(eventName, payload, ct);
    }

    public async Task SendToUserAsync<T>(
        Guid userId,
        string eventName,
        T payload,
        CancellationToken ct = default) where T : class
    {
        _logger.LogDebug("Sending {Event} to user {UserId}", eventName, userId);
        await _hubContext.Clients.Group(NotificationExtensions.UserGroup(userId)).SendAsync(eventName, payload, ct);
    }

    public async Task SendToAllAsync<T>(
        string eventName,
        T payload,
        CancellationToken ct = default) where T : class
    {
        _logger.LogDebug("Sending {Event} to all clients", eventName);
        await _hubContext.Clients.All.SendAsync(eventName, payload, ct);
    }
}
