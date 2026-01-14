using Microsoft.Extensions.Logging;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Cars.Commands.DeleteCar;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Tests.Handlers;

public class DeleteCarHandlerTests
{
    private readonly TestAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly ILogger<DeleteCarCommand> _logger = Substitute.For<ILogger<DeleteCarCommand>>();
    private readonly Guid _testUserId = Guid.NewGuid();

    public DeleteCarHandlerTests()
    {
        _dbContext = new TestAppDbContext();
        _currentUser.UserId.Returns(_testUserId);
    }

    [Fact]
    public async Task HandleAsync_WithValidCar_SoftDeletesCar()
    {
        // Arrange
        var car = Car.Create(_testUserId, "Model 3", "Tesla", _testUserId);
        _dbContext.Cars.Add(car);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteCarCommand(car.Id);

        // Act
        await DeleteCarHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _cache,
            _logger,
            CancellationToken.None);

        // Assert
        var deletedCar = await _dbContext.Cars.FindAsync(car.Id);
        Assert.True(deletedCar!.IsDeleted);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentCar_ThrowsNotFound()
    {
        // Arrange
        var command = new DeleteCarCommand(Guid.NewGuid());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            DeleteCarHandler.HandleAsync(
                command,
                _dbContext,
                _currentUser,
                _cache,
                _logger,
                CancellationToken.None));

        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_WithOtherUsersCar_ThrowsForbidden()
    {
        // Arrange - Car owned by different user
        var otherUserId = Guid.NewGuid();
        var car = Car.Create(otherUserId, "Model S", "Tesla", otherUserId);
        _dbContext.Cars.Add(car);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteCarCommand(car.Id);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            DeleteCarHandler.HandleAsync(
                command,
                _dbContext,
                _currentUser,
                _cache,
                _logger,
                CancellationToken.None));

        Assert.Contains("own", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
