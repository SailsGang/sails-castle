namespace SailsEnergy.Application.Notifications;

public static class NotificationEvents
{
    // Energy
    public const string EnergyLogged = "EnergyLogged";

    // Periods
    public const string PeriodStarted = "PeriodStarted";
    public const string PeriodClosed = "PeriodClosed";

    // Tariff
    public const string TariffChanged = "TariffChanged";

    // Gangs
    public const string GangDeleted = "GangDeleted";

    // Cars
    public const string CarAddedToGang = "CarAddedToGang";
    public const string CarRemovedFromGang = "CarRemovedFromGang";

    // Members
    public const string MemberJoined = "MemberJoined";
    public const string MemberLeft = "MemberLeft";
    public const string MemberKicked = "MemberKicked";
    public const string MemberRoleChanged = "MemberRoleChanged";

    // Invites
    public const string InviteReceived = "InviteReceived";

    // System
    public const string SystemAnnouncement = "SystemAnnouncement";
}
