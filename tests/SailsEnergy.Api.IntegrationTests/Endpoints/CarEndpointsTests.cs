using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SailsEnergy.Api.IntegrationTests.Fixtures;
using SailsEnergy.Api.IntegrationTests.Helpers;
using SailsEnergy.Application.Features.Cars.Commands.CreateCar;

namespace SailsEnergy.Api.IntegrationTests.Endpoints;

[Collection("Integration")]
public class CarEndpointsTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CarEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCar_WithAuthentication_ReturnsCreated()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        var command = new CreateCarCommand(
            "My Tesla",
            "Model 3",
            "Tesla",
            $"ABC{Guid.NewGuid():N}"[..7].ToUpper(),
            75.0m);

        // Act
        var response = await _client.PostAsJsonAsync("/api/cars", command);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created,
            $"Response: {responseBody}");
    }

    [Fact]
    public async Task CreateCar_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var freshClient = _factory.CreateClient();
        var command = new CreateCarCommand("My Car", "Model S", "Tesla", "XYZ1234", 100m);

        // Act
        var response = await freshClient.PostAsJsonAsync("/api/cars", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyCars_WithAuthentication_ReturnsOk()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Create a car first
        var createCommand = new CreateCarCommand(
            "My BMW",
            "i4",
            "BMW",
            $"CAR{Guid.NewGuid():N}"[..7].ToUpper(),
            83.9m);
        await _client.PostAsJsonAsync("/api/cars", createCommand);

        // Act - GET /api/cars returns my cars (not /api/cars/my)
        var response = await _client.GetAsync("/api/cars");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCar_WithValidId_ReturnsOk()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Create car
        var createCommand = new CreateCarCommand(
            "My Audi",
            "e-tron",
            "Audi",
            $"GET{Guid.NewGuid():N}"[..7].ToUpper(),
            95m);
        var createResponse = await _client.PostAsJsonAsync("/api/cars", createCommand);
        var carResponse = await createResponse.Content.ReadFromJsonAsync<CreateCarResponse>();

        // Act
        var response = await _client.GetAsync($"/api/cars/{carResponse!.CarId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteCar_AsOwner_ReturnsNoContent()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Create car
        var createCommand = new CreateCarCommand(
            "My VW",
            "ID.4",
            "Volkswagen",
            $"DEL{Guid.NewGuid():N}"[..7].ToUpper(),
            77m);
        var createResponse = await _client.PostAsJsonAsync("/api/cars", createCommand);
        var carResponse = await createResponse.Content.ReadFromJsonAsync<CreateCarResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/cars/{carResponse!.CarId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private record CreateCarResponse(Guid CarId);
}
