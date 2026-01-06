namespace SailsEnergy.Application.Abstractions;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string token);
}
