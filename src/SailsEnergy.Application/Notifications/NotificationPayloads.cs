namespace SailsEnergy.Application.Notifications;

public sealed record EnergyLoggedPayload(
    Guid GangId,
    Guid PeriodId,
    Guid EnergyLogId,
    decimal EnergyKwh,
    DateTimeOffset Timestamp);

public sealed record PeriodStartedPayload(
    Guid GangId,
    Guid PeriodId,
    DateTimeOffset StartedAt);

public sealed record PeriodClosedPayload(
    Guid GangId,
    string GangName,
    Guid PeriodId,
    decimal TotalEnergyKwh,
    decimal TotalCost,
    string Currency,
    DateTimeOffset ClosedAt);

public sealed record TariffChangedPayload(
    Guid GangId,
    Guid TariffId,
    decimal NewPrice,
    string Currency,
    DateTimeOffset ChangedAt);

public sealed record MemberJoinedPayload(
    Guid GangId,
    Guid UserId,
    string DisplayName,
    DateTimeOffset JoinedAt);

public sealed record MemberLeftPayload(
    Guid GangId,
    Guid UserId,
    DateTimeOffset LeftAt);

public sealed record InviteReceivedPayload(
    Guid GangId,
    string GangName,
    string InviteCode,
    DateTimeOffset ExpiresAt);

public sealed record MemberRoleChangedPayload(
    Guid GangId,
    Guid UserId,
    string NewRole,
    DateTimeOffset ChangedAt);

public sealed record MemberKickedPayload(
    Guid GangId,
    Guid UserId,
    string? Reason,
    DateTimeOffset KickedAt);

public sealed record GangDeletedPayload(
    Guid GangId,
    string GangName,
    DateTimeOffset DeletedAt);

public sealed record CarAddedToGangPayload(
    Guid GangId,
    Guid GangCarId,
    Guid CarId,
    string CarName,
    Guid AddedByUserId,
    DateTimeOffset AddedAt);

public sealed record CarRemovedFromGangPayload(
    Guid GangId,
    Guid CarId,
    string CarName,
    Guid RemovedByUserId,
    DateTimeOffset RemovedAt);
