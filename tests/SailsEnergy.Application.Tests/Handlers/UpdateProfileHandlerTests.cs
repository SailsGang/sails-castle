using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Users.Commands.UpdateProfile;
using SailsEnergy.Application.Tests;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Tests.Handlers;

public class UpdateProfileHandlerTests
{
    private readonly TestAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly Guid _testUserId = Guid.NewGuid();

    public UpdateProfileHandlerTests()
    {
        _dbContext = new TestAppDbContext();
        _currentUser.UserId.Returns(_testUserId);
    }

    [Fact]
    public async Task HandleAsync_WithDisplayName_UpdatesDisplayName()
    {
        // Arrange
        var profile = UserProfile.Create(_testUserId, "OldName", _testUserId);
        _dbContext.UserProfiles.Add(profile);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProfileCommand("NewDisplayName", null, null);

        // Act
        await UpdateProfileHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _cache,
            CancellationToken.None);

        // Assert
        var updated = _dbContext.UserProfiles.First(p => p.IdentityId == _testUserId);
        Assert.Equal("NewDisplayName", updated.DisplayName);
    }

    [Fact]
    public async Task HandleAsync_WithFirstAndLastName_UpdatesName()
    {
        // Arrange
        var profile = UserProfile.Create(_testUserId, "DisplayName", _testUserId);
        _dbContext.UserProfiles.Add(profile);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProfileCommand(null, "NewFirst", "NewLast");

        // Act
        await UpdateProfileHandler.HandleAsync(
            command,
            _dbContext,
            _currentUser,
            _cache,
            CancellationToken.None);

        // Assert
        var updated = _dbContext.UserProfiles.First(p => p.IdentityId == _testUserId);
        Assert.Equal("NewFirst", updated.FirstName);
        Assert.Equal("NewLast", updated.LastName);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentProfile_ThrowsNotFound()
    {
        // Arrange - No profile for user
        var command = new UpdateProfileCommand("Name", null, null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            UpdateProfileHandler.HandleAsync(
                command,
                _dbContext,
                _currentUser,
                _cache,
                CancellationToken.None));

        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
