using System.ComponentModel.DataAnnotations;

namespace SailsEnergy.Application.Features.Auth.Commands;

public record RegisterCommand(
    [Required] string Email,
    [Required] string Password,
    [Required] string ConfirmPassword,
    [Required] string DisplayName,
    string? FirstName,
    string? LastName);
