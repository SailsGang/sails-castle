namespace SailsEnergy.Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string? DisplayName,
    string? FirstName,
    string? LastName);
