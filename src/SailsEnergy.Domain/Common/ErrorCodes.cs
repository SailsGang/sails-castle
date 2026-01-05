namespace SailsEnergy.Domain.Common;

public static class ErrorCodes
{
    // Auth
    public const string Unauthorized = "UNAUTHORIZED";

    // Gang
    public const string GangNotFound = "GANG_NOT_FOUND";
    public const string AlreadyGangMember = "ALREADY_GANG_MEMBER";

    // Period
    public const string PeriodAlreadyClosed = "PERIOD_ALREADY_CLOSED";
    public const string NoPeriodActive = "NO_ACTIVE_PERIOD";

    // Energy Log
    public const string EditWindowExpired = "EDIT_WINDOW_EXPIRED";
    public const string InvalidEnergy = "INVALID_ENERGY";

    // Tariff
    public const string InvalidPrice = "INVALID_PRICE";
}
