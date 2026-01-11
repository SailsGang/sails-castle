using FluentAssertions;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;
using SailsEnergy.Infrastructure.Services;

namespace SailsEnergy.Application.Tests.Services;

public class GangAuthorizationServiceTests
{
    private readonly ICurrentUserService _currentUser;
    private readonly GangAuthorizationService _service;
    private readonly TestAppDbContext _dbContext;

    public GangAuthorizationServiceTests()
    {
        _currentUser = Substitute.For<ICurrentUserService>();
        _dbContext = new TestAppDbContext();
        _service = new GangAuthorizationService(_dbContext, _currentUser);
    }

    [Fact]
    public async Task RequireMembershipAsync_WhenNotAuthenticated_ThrowsUnauthorized()
    {
        _currentUser.UserId.Returns((Guid?)null);

        var act = () => _service.RequireMembershipAsync(Guid.NewGuid());

        var exception = await act.Should().ThrowAsync<BusinessRuleException>();
        exception.Which.Code.Should().Be(ErrorCodes.Unauthorized);
    }

    [Fact]
    public async Task RequireMembershipAsync_WhenNotMember_ThrowsForbidden()
    {
        var gangId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var act = () => _service.RequireMembershipAsync(gangId);

        var exception = await act.Should().ThrowAsync<BusinessRuleException>();
        exception.Which.Code.Should().Be(ErrorCodes.Forbidden);
    }

    [Fact]
    public async Task RequireMembershipAsync_WhenMember_ReturnsSuccess()
    {
        var gangId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var member = GangMember.Create(gangId, userId, MemberRole.Member, userId);
        _dbContext.GangMembers.Add(member);
        await _dbContext.SaveChangesAsync();

        var result = await _service.RequireMembershipAsync(gangId);

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task RequireAdminAsync_WhenMemberRole_ThrowsForbidden()
    {
        var gangId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var member = GangMember.Create(gangId, userId, MemberRole.Member, userId);
        _dbContext.GangMembers.Add(member);
        await _dbContext.SaveChangesAsync();

        var act = () => _service.RequireAdminAsync(gangId);

        var exception = await act.Should().ThrowAsync<BusinessRuleException>();
        exception.Which.Code.Should().Be(ErrorCodes.Forbidden);
    }

    [Theory]
    [InlineData(MemberRole.Owner)]
    [InlineData(MemberRole.Admin)]
    public async Task RequireAdminAsync_WhenOwnerOrAdmin_ReturnsSuccess(MemberRole role)
    {
        var gangId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var member = GangMember.Create(gangId, userId, role, userId);
        _dbContext.GangMembers.Add(member);
        await _dbContext.SaveChangesAsync();

        var result = await _service.RequireAdminAsync(gangId);

        result.Should().NotBeNull();
        result.Role.Should().Be(role);
    }

    [Fact]
    public async Task RequireOwnerAsync_WhenAdmin_ThrowsForbidden()
    {
        var gangId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var member = GangMember.Create(gangId, userId, MemberRole.Admin, userId);
        _dbContext.GangMembers.Add(member);
        await _dbContext.SaveChangesAsync();

        var act = () => _service.RequireOwnerAsync(gangId);

        var exception = await act.Should().ThrowAsync<BusinessRuleException>();
        exception.Which.Code.Should().Be(ErrorCodes.Forbidden);
    }

    [Fact]
    public async Task RequireOwnerAsync_WhenOwner_ReturnsSuccess()
    {
        var gangId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var member = GangMember.Create(gangId, userId, MemberRole.Owner, userId);
        _dbContext.GangMembers.Add(member);
        await _dbContext.SaveChangesAsync();

        var result = await _service.RequireOwnerAsync(gangId);

        result.Should().NotBeNull();
        result.Role.Should().Be(MemberRole.Owner);
    }

    [Fact]
    public async Task IsMemberAsync_WhenNotAuthenticated_ReturnsFalse()
    {
        _currentUser.UserId.Returns((Guid?)null);

        var result = await _service.IsMemberAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsMemberAsync_WhenMember_ReturnsTrue()
    {
        var gangId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var member = GangMember.Create(gangId, userId, MemberRole.Member, userId);
        _dbContext.GangMembers.Add(member);
        await _dbContext.SaveChangesAsync();

        var result = await _service.IsMemberAsync(gangId);

        result.Should().BeTrue();
    }
}
