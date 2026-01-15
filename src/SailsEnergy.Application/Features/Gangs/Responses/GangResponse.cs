namespace SailsEnergy.Application.Features.Gangs.Responses;

/// <summary>
/// Gang details response
/// </summary>
public sealed record GangResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid OwnerId,
    DateTimeOffset CreatedAt);

/// <summary>
/// Gang list item with user's role
/// </summary>
public sealed record GangListItem(
    Guid Id,
    string Name,
    string? Description,
    Guid OwnerId,
    string Role,
    DateTimeOffset CreatedAt);

/// <summary>
/// Gang list item with gang members
/// </summary>
public sealed record GangMemberResponse(
    Guid Id,
    Guid UserId,
    string DisplayName,
    string Email,
    string Role,
    DateTimeOffset JoinedAt);

/// <summary>
/// Gang list item with gang cars
/// </summary>
public sealed record GangCarResponse(
    Guid Id,
    Guid CarId,
    string CarName,
    string? LicensePlate,
    decimal BatteryCapacityKwh,
    Guid OwnerId,
    DateTimeOffset AddedAt);
