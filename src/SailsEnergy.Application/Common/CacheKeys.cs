namespace SailsEnergy.Application.Common;

public static class CacheKeys
{
    public static string Gang(Guid gangId) => $"gang:{gangId}";
    public static string GangMembers(Guid gangId) => $"gang:{gangId}:members";
    public static string GangCars(Guid gangId) => $"gang:{gangId}:cars";
    public static string UserProfile(Guid userId) => $"user:{userId}:profile";
    public static string UserGangs(Guid userId) => $"user:{userId}:gangs";
    public static string Car(Guid carId) => $"car:{carId}";
}
