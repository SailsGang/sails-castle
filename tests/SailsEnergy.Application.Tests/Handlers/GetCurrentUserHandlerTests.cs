using Microsoft.Extensions.Logging;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Users.Queries.GetCurrentUser;
using SailsEnergy.Application.Features.Users.Responses;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Tests.Handlers;

public class GetCurrentUserHandlerTests
{
    private readonly TestAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly ILogger<GetCurrentUserQuery> _logger = Substitute.For<ILogger<GetCurrentUserQuery>>();
    private readonly Guid _testUserId = Guid.NewGuid();

    public GetCurrentUserHandlerTests()
    {
        _dbContext = new TestAppDbContext();
        _currentUser.UserId.Returns(_testUserId);
        _currentUser.Email.Returns("test@example.com");
    }

    [Fact]
    public async Task HandleAsync_WithExistingProfile_ReturnsProfile()
    {
        // Arrange
        var profile = UserProfile.Create(_testUserId, "TestUser", _testUserId);
        _dbContext.UserProfiles.Add(profile);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        _cache.GetEntityAsync<UserProfileResponse>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((UserProfileResponse?)null);

        var query = new GetCurrentUserQuery();

        // Act
        var result = await GetCurrentUserHandler.HandleAsync(
            query,
            _dbContext,
            _currentUser,
            _cache,
            _logger,
            CancellationToken.None);

        // Assert
        Assert.Equal("TestUser", result.DisplayName);
        Assert.Null(result.FirstName); // Not set by Create
        Assert.Null(result.LastName);  // Not set by Create
    }

    [Fact]
    public async Task HandleAsync_WithCachedProfile_ReturnsCached()
    {
        // Arrange
        var cachedResponse = new UserProfileResponse(
            Guid.NewGuid(),
            "test@example.com",
            "CachedUser",
            "Cached",
            "User",
            DateTimeOffset.UtcNow);

        _cache.GetEntityAsync<UserProfileResponse>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(cachedResponse);

        var query = new GetCurrentUserQuery();

        // Act
        var result = await GetCurrentUserHandler.HandleAsync(
            query,
            _dbContext,
            _currentUser,
            _cache,
            _logger,
            CancellationToken.None);

        // Assert
        Assert.Equal("CachedUser", result.DisplayName);
    }

    [Fact]
    public async Task HandleAsync_WithNoProfile_ThrowsNotFound()
    {
        // Arrange
        _cache.GetEntityAsync<UserProfileResponse>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((UserProfileResponse?)null);

        var query = new GetCurrentUserQuery();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            GetCurrentUserHandler.HandleAsync(
                query,
                _dbContext,
                _currentUser,
                _cache,
                _logger,
                CancellationToken.None));

        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
