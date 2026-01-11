using SailsEnergy.Application.Abstractions;

namespace SailsEnergy.Application.Notifications;

public static class NotificationExtensions
{
    public static string GangGroup(Guid gangId) => $"gang:{gangId}";
    public static string UserGroup(Guid userId) => $"user:{userId}";
    public static Task NotifyGangAsync<T>(
        this IRealtimeNotificationService service,
        Guid gangId,
        string eventName,
        T payload,
        CancellationToken ct = default) where T : class =>
        service.SendToGroupAsync(GangGroup(gangId), eventName, payload, ct);
}
