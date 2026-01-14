using System.Net.Http.Headers;
using System.Net.Http.Json;
using SailsEnergy.Application.Features.Auth.Commands.Register;

namespace SailsEnergy.Api.IntegrationTests.Helpers;

public static class AuthHelper
{
    public record TestUser(string Email, string Password, string DisplayName, string AccessToken, Guid UserId);

    public static async Task<TestUser> CreateAuthenticatedUserAsync(
        HttpClient client,
        string? email = null,
        string? displayName = null)
    {
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        email ??= $"testuser{uniqueId}@example.com";
        displayName ??= $"TestUser{uniqueId}";
        var password = "Password123!";

        var registerCommand = new RegisterCommand(
            email,
            password,
            password,
            displayName,
            "Test",
            "User");

        var response = await client.PostAsJsonAsync("/api/auth/register", registerCommand);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        return new TestUser(
            email, 
            password, 
            displayName, 
            authResponse!.AccessToken,
            authResponse.UserId);
    }

    public static void SetAuthToken(HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);
    }

    private record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTimeOffset ExpiresAt,
        Guid UserId,
        string Email,
        string DisplayName);
}
