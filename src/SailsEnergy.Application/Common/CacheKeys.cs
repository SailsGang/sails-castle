namespace SailsEnergy.Application.Common;

public static class CacheKeys
{
    public static string Gang(Guid gangId) => $"gang:{gangId}";
    public static string GangMembers(Guid gangId) => $"gang:{gangId}:members";
    public static string GangCars(Guid gangId) => $"gang:{gangId}:cars";
    public static string UserProfile(Guid userId) => $"user:{userId}:profile";
    public static string UserGangs(Guid userId) => $"user:{userId}:gangs";
    public static string Car(Guid carId) => $"car:{carId}";
    public static string GangTariff(Guid gangId) => $"gang:{gangId}:current-tariff";
    public static string GangActivePeriod(Guid gangId) => $"gang:{gangId}:active-period";
    public static string GangEnergyLogs(Guid gangId, Guid periodId) => $"gang:{gangId}:period:{periodId}:logs";
    public static string UserEnergyLogs(Guid userId) => $"user:{userId}:energy-logs";
}
