using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SailsEnergy.Api.IntegrationTests.Fixtures;
using SailsEnergy.Application.Features.Auth.Commands.Login;
using SailsEnergy.Application.Features.Auth.Commands.Register;

namespace SailsEnergy.Api.IntegrationTests.Endpoints;

[Collection("Integration")]
public class AuthEndpointsTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsSuccess()
    {
        // Arrange - DisplayName must be alphanumeric (used as username)
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var command = new RegisterCommand(
            $"test{uniqueId}@example.com",
            "Password123!",
            "Password123!",
            $"TestUser{uniqueId}",  // No spaces - used as username
            "Test",
            "User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", command);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created, 
            $"Response body: {responseBody}");
        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        content.Should().NotBeNull();
        content!.AccessToken.Should().NotBeNullOrEmpty();
        content.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var command = new RegisterCommand(
            "invalid-email",
            "Password123!",
            "Password123!",
            "TestUser123",
            "Test",
            "User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange - Register first
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var email = $"login{uniqueId}@example.com";
        var password = "Password123!";
        
        var registerCommand = new RegisterCommand(
            email,
            password,
            password,
            $"LoginTestUser{uniqueId}",  // No spaces
            "Login",
            "Test");

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerCommand);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created, 
            $"Registration failed: {registerBody}");

        // Act - Login
        var loginCommand = new LoginCommand(email, password);
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, 
            $"Login failed: {responseBody}");
        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        content.Should().NotBeNull();
        content!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var command = new LoginCommand("nonexistent@example.com", "WrongPassword!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", command);

        // Assert - Endpoint returns 422 for invalid credentials
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTimeOffset ExpiresAt,
        Guid UserId,
        string Email,
        string DisplayName);
}
