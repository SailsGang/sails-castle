using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SailsEnergy.Api.IntegrationTests.Fixtures;
using SailsEnergy.Api.IntegrationTests.Helpers;
using SailsEnergy.Application.Features.Cars.Commands.CreateCar;
using SailsEnergy.Application.Features.EnergyLogs.Commands.LogEnergy;
using SailsEnergy.Application.Features.Gangs.Commands.CreateGang;

namespace SailsEnergy.Api.IntegrationTests.Endpoints;

[Collection("Integration")]
public class EnergyEndpointsTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public EnergyEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LogEnergy_WithValidData_ReturnsCreated()
    {
        // Arrange - Need user, gang, car
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Create gang
        var gangCommand = new CreateGangCommand($"EnergyGang{Guid.NewGuid():N}"[..15], "Test");
        var gangResponse = await _client.PostAsJsonAsync("/api/gangs", gangCommand);
        var gang = await gangResponse.Content.ReadFromJsonAsync<GangResponse>();

        // Create car
        var carCommand = new CreateCarCommand(
            "EnergyCar",
            "Model Y",
            "Tesla",
            $"ENE{Guid.NewGuid():N}"[..7].ToUpper(),
            75m);
        var carResponse = await _client.PostAsJsonAsync("/api/cars", carCommand);
        var car = await carResponse.Content.ReadFromJsonAsync<CarResponse>();

        // Add car to gang - POST /api/gangs/{gangId}/cars with CarId in body
        // Response now includes GangCarId
        var addCarRequest = new AddCarToGangRequest(car!.CarId);
        var addCarResponse = await _client.PostAsJsonAsync($"/api/gangs/{gang!.GangId}/cars", addCarRequest);
        var addCarBody = await addCarResponse.Content.ReadAsStringAsync();
        addCarResponse.StatusCode.Should().Be(HttpStatusCode.Created,
            $"AddCarToGang failed: {addCarBody}");
        var addCarResult = await addCarResponse.Content.ReadFromJsonAsync<AddCarToGangResponse>();

        // Act - Log energy using GangCarId from AddCarToGang response
        var logCommand = new LogEnergyCommand(
            gang.GangId,
            addCarResult!.GangCarId,  // Use GangCarId from response
            50.5m,
            DateTimeOffset.UtcNow,
            "Test charging session");
        var response = await _client.PostAsJsonAsync("/api/energy", logCommand);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created,
            $"Response: {responseBody}");
    }

    [Fact]
    public async Task LogEnergy_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var freshClient = _factory.CreateClient();
        var command = new LogEnergyCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10m,
            DateTimeOffset.UtcNow,
            null);

        // Act
        var response = await freshClient.PostAsJsonAsync("/api/energy", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetEnergyLogs_ForGang_ReturnsOk()
    {
        // Arrange
        var user = await AuthHelper.CreateAuthenticatedUserAsync(_client);
        AuthHelper.SetAuthToken(_client, user.AccessToken);

        // Create gang
        var gangCommand = new CreateGangCommand($"LogsGang{Guid.NewGuid():N}"[..15], "Test");
        var gangResponse = await _client.PostAsJsonAsync("/api/gangs", gangCommand);
        var gang = await gangResponse.Content.ReadFromJsonAsync<GangResponse>();

        // Act - Use query params: /api/energy?gangId={id}&page=1&pageSize=10
        var response = await _client.GetAsync($"/api/energy?gangId={gang!.GangId}&page=1&pageSize=10");
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK,
            $"Response: {responseBody}");
    }

    private record GangResponse(Guid GangId, string Name, string Description);
    private record CarResponse(Guid CarId);
    private record AddCarToGangRequest(Guid CarId);
    private record AddCarToGangResponse(Guid GangCarId);
}
