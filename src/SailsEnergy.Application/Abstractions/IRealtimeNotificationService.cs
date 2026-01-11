namespace SailsEnergy.Application.Abstractions;

public interface IRealtimeNotificationService
{
    /// <summary>
    /// Send notification to a specific group (e.g., gang members)
    /// </summary>
    Task SendToGroupAsync<T>(string groupName, string eventName, T payload, CancellationToken ct = default) where T : class;

    /// <summary>
    /// Send notification to a specific user
    /// </summary>
    Task SendToUserAsync<T>(Guid userId, string eventName, T payload, CancellationToken ct = default) where T : class;

    /// <summary>
    /// Send notification to all connected clients
    /// </summary>
    Task SendToAllAsync<T>(string eventName, T payload, CancellationToken ct = default) where T : class;
}
