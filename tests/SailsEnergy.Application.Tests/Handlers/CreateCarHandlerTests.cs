using Microsoft.Extensions.Logging;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Cars.Commands.CreateCar;
using SailsEnergy.Application.Tests;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Application.Tests.Handlers;

public class CreateCarHandlerTests
{
    private readonly TestAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<CreateCarCommand> _logger = Substitute.For<ILogger<CreateCarCommand>>();
    private readonly Guid _testUserId = Guid.NewGuid();

    public CreateCarHandlerTests()
    {
        _dbContext = new TestAppDbContext();
        _currentUser.UserId.Returns(_testUserId);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesCar()
    {
        // Arrange
        var command = new CreateCarCommand(
            "My Tesla",
            "Model 3",
            "Tesla",
            "ABC123",
            75m);

        // Act
        var result = await CreateCarHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _logger,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.CarId);
        var savedCar = await _dbContext.Cars.FindAsync(result.CarId);
        Assert.NotNull(savedCar);
        Assert.Equal("Model 3", savedCar.Model);
        Assert.Equal("Tesla", savedCar.Manufacturer);
        Assert.Equal("My Tesla", savedCar.Name);
        Assert.Equal("ABC123", savedCar.LicensePlate);
    }

    [Fact]
    public async Task HandleAsync_WithoutOptionalFields_CreatesCar()
    {
        // Arrange - Only required fields
        var command = new CreateCarCommand(
            null,          // Name - optional
            "Model S",
            "Tesla",
            null,          // LicensePlate - optional
            null);         // BatteryCapacity - optional

        // Act
        var result = await CreateCarHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _logger,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.CarId);
        var savedCar = await _dbContext.Cars.FindAsync(result.CarId);
        Assert.NotNull(savedCar);
        Assert.Equal("Model S", savedCar.Model);
        // Name is optional and not set
        Assert.Null(savedCar.LicensePlate);
    }

    [Fact]
    public async Task HandleAsync_SetsUserAsOwner()
    {
        // Arrange
        var command = new CreateCarCommand(
            "My Car",
            "ID.4",
            "Volkswagen",
            null,
            77m);

        // Act
        var result = await CreateCarHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _logger,
            CancellationToken.None);

        // Assert
        var savedCar = await _dbContext.Cars.FindAsync(result.CarId);
        Assert.NotNull(savedCar);
        Assert.Equal(_testUserId, savedCar.OwnerId);
    }
}
