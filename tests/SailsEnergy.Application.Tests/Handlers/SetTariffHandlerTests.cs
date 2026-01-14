using Microsoft.Extensions.Logging;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Tariffs.Commands.SetTariff;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Application.Tests.Handlers;

public class SetTariffHandlerTests
{
    private readonly TestAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IGangAuthorizationService _gangAuth = Substitute.For<IGangAuthorizationService>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly ILogger<SetTariffCommand> _logger = Substitute.For<ILogger<SetTariffCommand>>();
    private readonly IRealtimeNotificationService _notificationService = Substitute.For<IRealtimeNotificationService>();
    private readonly Guid _testUserId = Guid.NewGuid();

    public SetTariffHandlerTests()
    {
        _dbContext = new TestAppDbContext();
        _currentUser.UserId.Returns(_testUserId);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesTariff()
    {
        // Arrange
        var gang = Gang.Create("TestGang", _testUserId, "Test");
        _dbContext.Gangs.Add(gang);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new SetTariffCommand(gang.Id, 0.15m, "UAH");

        // Act
        var result = await SetTariffHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _gangAuth,
            _cache,
            _logger,
            _notificationService,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(0.15m, result.PricePerKwh);
        Assert.Equal("UAH", result.Currency);
    }

    [Fact]
    public async Task HandleAsync_NotifiesGang()
    {
        // Arrange
        var gang = Gang.Create("TestGang", _testUserId, "Test");
        _dbContext.Gangs.Add(gang);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new SetTariffCommand(gang.Id, 0.20m, "USD");

        // Act
        await SetTariffHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _gangAuth,
            _cache,
            _logger,
            _notificationService,
            CancellationToken.None);

        // Assert - Notification was sent (via SendToGroupAsync)
        await _notificationService.Received(1).SendToGroupAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_InvalidatesCacheForGang()
    {
        // Arrange
        var gang = Gang.Create("TestGang", _testUserId, "Test");
        _dbContext.Gangs.Add(gang);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new SetTariffCommand(gang.Id, 0.25m, "EUR");

        // Act
        await SetTariffHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _gangAuth,
            _cache,
            _logger,
            _notificationService,
            CancellationToken.None);

        // Assert
        await _cache.Received(1).RemoveAsync(
            Arg.Is<string>(s => s.Contains(gang.Id.ToString())),
            Arg.Any<CancellationToken>());
    }
}
