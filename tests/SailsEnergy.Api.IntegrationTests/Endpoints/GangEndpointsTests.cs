using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SailsEnergy.Api.IntegrationTests.Fixtures;
using SailsEnergy.Api.IntegrationTests.Helpers;
using SailsEnergy.Application.Features.Gangs.Commands.CreateGang;

namespace SailsEnergy.Api.IntegrationTests.Endpoints;

[Collection("Integration")]
public class GangEndpointsTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GangEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateGang_WithAuthentication_ReturnsCreated()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        var command = new CreateGangCommand($"TestGang{Guid.NewGuid():N}"[..20], "A test gang");

        // Act
        var response = await _client.PostAsJsonAsync("/api/gangs", command);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created,
            $"Response: {responseBody}");
    }

    [Fact]
    public async Task CreateGang_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange - No auth token
        var freshClient = _factory.CreateClient();
        var command = new CreateGangCommand("NoAuthGang", "Should fail");

        // Act
        var response = await freshClient.PostAsJsonAsync("/api/gangs", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyGangs_WithAuthentication_ReturnsOk()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Create a gang first
        var createCommand = new CreateGangCommand($"MyGang{Guid.NewGuid():N}"[..15], "Test");
        await _client.PostAsJsonAsync("/api/gangs", createCommand);

        // Act - GET /api/gangs returns my gangs
        var response = await _client.GetAsync("/api/gangs");
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK,
            $"Response: {responseBody}");
    }

    [Fact]
    public async Task GetGang_WithValidId_ReturnsOk()
    {
        // Arrange 
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Create gang and get its ID
        var createCommand = new CreateGangCommand($"GetGang{Guid.NewGuid():N}"[..15], "Test");
        var createResponse = await _client.PostAsJsonAsync("/api/gangs", createCommand);
        var gangResponse = await createResponse.Content.ReadFromJsonAsync<CreateGangResponse>();

        // Act
        var response = await _client.GetAsync($"/api/gangs/{gangResponse!.GangId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGang_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Act
        var response = await _client.GetAsync($"/api/gangs/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateGang_AsOwner_ReturnsOk()
    {
        // Arrange  
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Create gang
        var createCommand = new CreateGangCommand($"UpdateMe{Guid.NewGuid():N}"[..15], "Original");
        var createResponse = await _client.PostAsJsonAsync("/api/gangs", createCommand);
        var gangResponse = await createResponse.Content.ReadFromJsonAsync<CreateGangResponse>();

        // Act - Update
        var updateRequest = new { Name = "UpdatedName", Description = "Updated description" };
        var response = await _client.PutAsJsonAsync($"/api/gangs/{gangResponse!.GangId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteGang_AsOwner_ReturnsNoContent()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Create gang
        var createCommand = new CreateGangCommand($"DeleteMe{Guid.NewGuid():N}"[..15], "ToDelete");
        var createResponse = await _client.PostAsJsonAsync("/api/gangs", createCommand);
        var gangResponse = await createResponse.Content.ReadFromJsonAsync<CreateGangResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/gangs/{gangResponse!.GangId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private record CreateGangResponse(Guid GangId);
}
