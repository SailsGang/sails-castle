using Microsoft.Extensions.Logging;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Gangs.Commands.CreateGang;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Tests.Handlers;

public class CreateGangHandlerTests
{
    private readonly TestAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly ILogger<CreateGangCommand> _logger = Substitute.For<ILogger<CreateGangCommand>>();
    private readonly Guid _testUserId = Guid.NewGuid();

    public CreateGangHandlerTests()
    {
        _dbContext = new TestAppDbContext();
        _currentUser.UserId.Returns(_testUserId);
    }

    [Fact(Skip = "Requires DB transaction support - covered by integration tests")]
    public async Task HandleAsync_WithValidCommand_CreatesGang()
    {
        // Arrange
        var command = new CreateGangCommand("TestGang", "A test gang");

        // Act
        var result = await CreateGangHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _cache,
            _logger,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.GangId);
        var savedGang = await _dbContext.Gangs.FindAsync(result.GangId);
        Assert.NotNull(savedGang);
        Assert.Equal("TestGang", savedGang.Name);
        Assert.Equal("A test gang", savedGang.Description);
    }

    [Fact(Skip = "Requires DB transaction support - covered by integration tests")]
    public async Task HandleAsync_AddsUserAsOwner()
    {
        // Arrange
        var command = new CreateGangCommand("OwnerGang", null);

        // Act
        var result = await CreateGangHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _cache,
            _logger,
            CancellationToken.None);

        // Assert
        var member = _dbContext.GangMembers
            .FirstOrDefault(m => m.GangId == result.GangId && m.UserId == _testUserId);
        Assert.NotNull(member);
        Assert.Equal(MemberRole.Owner, member.Role);
    }

    [Fact(Skip = "Requires DB transaction support - covered by integration tests")]
    public async Task HandleAsync_CreatesActivePeriod()
    {
        // Arrange
        var command = new CreateGangCommand("PeriodGang", null);

        // Act
        var result = await CreateGangHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _cache,
            _logger,
            CancellationToken.None);

        // Assert
        var period = _dbContext.Periods.FirstOrDefault(p => p.GangId == result.GangId);
        Assert.NotNull(period);
        Assert.Equal(PeriodStatus.Active, period.Status);
    }

    [Fact(Skip = "Requires DB transaction support - covered by integration tests")]
    public async Task HandleAsync_CreatesDefaultTariff()
    {
        // Arrange
        var command = new CreateGangCommand("TariffGang", null);

        // Act
        var result = await CreateGangHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _cache,
            _logger,
            CancellationToken.None);

        // Assert
        var tariff = _dbContext.Tariffs.FirstOrDefault(t => t.GangId == result.GangId);
        Assert.NotNull(tariff);
        Assert.Equal(0m, tariff.PricePerKwh);
        Assert.Equal("UAH", tariff.Currency);
    }
}
