namespace SailsEnergy.Application.Features.Auth.Commands;

public record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string DisplayName,
    string? FirstName,
    string? LastName);
