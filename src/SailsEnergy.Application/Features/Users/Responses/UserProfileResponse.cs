namespace SailsEnergy.Application.Features.Users.Responses;

public sealed record UserProfileResponse(
    Guid Id,
    string Email,
    string DisplayName,
    string? FirstName,
    string? LastName,
    DateTimeOffset CreatedAt);
