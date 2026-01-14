using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SailsEnergy.Api.IntegrationTests.Fixtures;
using SailsEnergy.Api.IntegrationTests.Helpers;
using SailsEnergy.Application.Features.Users.Commands.UpdateProfile;

namespace SailsEnergy.Api.IntegrationTests.Endpoints;

[Collection("Integration")]
public class UserEndpointsTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UserEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCurrentUser_WithAuthentication_ReturnsOk()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/me");
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK,
            $"Response: {responseBody}");
    }

    [Fact]
    public async Task GetCurrentUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var freshClient = _factory.CreateClient();

        // Act
        var response = await freshClient.GetAsync("/api/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProfile_WithAuthentication_ReturnsNoContent()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // UpdateProfileCommand has DisplayName, FirstName, LastName
        var updateCommand = new UpdateProfileCommand(
            null,        // DisplayName - not changing
            "Updated",   // FirstName
            "Name");     // LastName

        // Act
        var response = await _client.PutAsJsonAsync("/api/me", updateCommand);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert - endpoint returns 204 NoContent on success
        response.StatusCode.Should().Be(HttpStatusCode.NoContent,
            $"Response: {responseBody}");
    }
}
