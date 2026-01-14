using Microsoft.Extensions.Logging;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.EnergyLogs.Commands.LogEnergy;
using SailsEnergy.Application.Tests;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Tests.Handlers;

public class LogEnergyHandlerTests
{
    private readonly TestAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly ILogger<LogEnergyCommand> _logger = Substitute.For<ILogger<LogEnergyCommand>>();
    private readonly IRealtimeNotificationService _notificationService = Substitute.For<IRealtimeNotificationService>();
    private readonly Guid _testUserId = Guid.NewGuid();

    public LogEnergyHandlerTests()
    {
        _dbContext = new TestAppDbContext();
        _currentUser.UserId.Returns(_testUserId);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_CreatesEnergyLog()
    {
        // Arrange - Create full setup: Gang, Member, Car, GangCar, Period, Tariff
        var (gang, gangCar, period, tariff) = await SetupGangWithCarAsync();

        _cache.GetOrCreateAsync<Tariff?>(
            Arg.Any<string>(),
            Arg.Any<Func<Task<Tariff?>>>(),
            Arg.Any<TimeSpan>(),
            Arg.Any<CancellationToken>())
            .Returns(tariff);

        var command = new LogEnergyCommand(
            gang.Id,
            gangCar.Id,
            50.5m,
            DateTimeOffset.UtcNow,
            "Test charging");

        // Act
        var result = await LogEnergyHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _cache,
            _logger,
            _notificationService,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.LogId);
        Assert.Equal(50.5m * tariff.PricePerKwh, result.CostUah);
    }

    [Fact]
    public async Task HandleAsync_WhenNotMember_ThrowsException()
    {
        // Arrange - Create gang but don't add user as member
        var gang = Gang.Create("TestGang", Guid.NewGuid(), "Test");
        _dbContext.Gangs.Add(gang);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new LogEnergyCommand(
            gang.Id,
            Guid.NewGuid(),
            10m,
            DateTimeOffset.UtcNow,
            null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            LogEnergyHandler.HandleAsync(
                command,
                _dbContext,
                _currentUser,
                _cache,
                _logger,
                _notificationService,
                CancellationToken.None));

        Assert.Contains("not a member", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_WhenNoActivePeriod_ThrowsException()
    {
        // Arrange - Gang with member but no active period
        var gang = Gang.Create("TestGang", _testUserId, "Test");
        _dbContext.Gangs.Add(gang);

        var member = GangMember.Create(gang.Id, _testUserId, MemberRole.Owner, _testUserId);
        _dbContext.GangMembers.Add(member);

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new LogEnergyCommand(
            gang.Id,
            Guid.NewGuid(),
            10m,
            DateTimeOffset.UtcNow,
            null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            LogEnergyHandler.HandleAsync(
                command,
                _dbContext,
                _currentUser,
                _cache,
                _logger,
                _notificationService,
                CancellationToken.None));

        Assert.Contains("no active period", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<(Gang, GangCar, Period, Tariff)> SetupGangWithCarAsync()
    {
        var gang = Gang.Create("TestGang", _testUserId, "Test");
        _dbContext.Gangs.Add(gang);

        var member = GangMember.Create(gang.Id, _testUserId, MemberRole.Owner, _testUserId);
        _dbContext.GangMembers.Add(member);

        var car = Car.Create(_testUserId, "Model 3", "Tesla", _testUserId);
        _dbContext.Cars.Add(car);

        var gangCar = GangCar.Create(gang.Id, car.Id, _testUserId);
        _dbContext.GangCars.Add(gangCar);

        var period = Period.Create(gang.Id, _testUserId);
        _dbContext.Periods.Add(period);

        var tariff = Tariff.Create(gang.Id, 0.15m, "UAH", _testUserId);
        _dbContext.Tariffs.Add(tariff);

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        return (gang, gangCar, period, tariff);
    }
}
