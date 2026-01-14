using Microsoft.Extensions.Logging;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Gangs.Commands.DeleteGang;
using SailsEnergy.Application.Tests;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Tests.Handlers;

public class DeleteGangHandlerTests
{
    private readonly TestAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IGangAuthorizationService _gangAuth = Substitute.For<IGangAuthorizationService>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly ILogger<DeleteGangCommand> _logger = Substitute.For<ILogger<DeleteGangCommand>>();
    private readonly IRealtimeNotificationService _notificationService = Substitute.For<IRealtimeNotificationService>();
    private readonly Guid _testUserId = Guid.NewGuid();

    public DeleteGangHandlerTests()
    {
        _dbContext = new TestAppDbContext();
        _currentUser.UserId.Returns(_testUserId);
    }

    [Fact]
    public async Task HandleAsync_AsOwner_SoftDeletesGang()
    {
        // Arrange
        var gang = Gang.Create("TestGang", _testUserId, "Test");
        _dbContext.Gangs.Add(gang);

        var member = GangMember.Create(gang.Id, _testUserId, MemberRole.Owner, _testUserId);
        _dbContext.GangMembers.Add(member);

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteGangCommand(gang.Id);

        // Act
        await DeleteGangHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _gangAuth,
            _cache,
            _logger,
            _notificationService,
            CancellationToken.None);

        // Assert
        var deletedGang = await _dbContext.Gangs.FindAsync(gang.Id);
        Assert.True(deletedGang!.IsDeleted);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentGang_ThrowsNotFound()
    {
        // Arrange
        var command = new DeleteGangCommand(Guid.NewGuid());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            DeleteGangHandler.HandleAsync(
                command,
                _dbContext,
                _currentUser,
                _gangAuth,
                _cache,
                _logger,
                _notificationService,
                CancellationToken.None));

        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_NotifiesAllMembers()
    {
        // Arrange
        var gang = Gang.Create("TestGang", _testUserId, "Test");
        _dbContext.Gangs.Add(gang);

        var owner = GangMember.Create(gang.Id, _testUserId, MemberRole.Owner, _testUserId);
        _dbContext.GangMembers.Add(owner);

        var otherUserId = Guid.NewGuid();
        var member = GangMember.Create(gang.Id, otherUserId, MemberRole.Member, _testUserId);
        _dbContext.GangMembers.Add(member);

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteGangCommand(gang.Id);

        // Act
        await DeleteGangHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _gangAuth,
            _cache,
            _logger,
            _notificationService,
            CancellationToken.None);

        // Assert - Both members should receive notification
        await _notificationService.Received(2).SendToUserAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }
}
