namespace SailsEnergy.Domain.Tests.Entities;

using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Events;
using SailsEnergy.Domain.ValueObjects;

public class GangMemberTests
{
    private readonly Guid _gangId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var member = GangMember.Create(_gangId, _userId, MemberRole.Member, _userId);

        // Assert
        member.GangId.Should().Be(_gangId);
        member.UserId.Should().Be(_userId);
        member.Role.Should().Be(MemberRole.Member);
        member.IsActive.Should().BeTrue();
        member.Id.Should().NotBeEmpty();
        member.CreatedBy.Should().Be(_userId);
    }

    [Theory]
    [InlineData(MemberRole.Owner)]
    [InlineData(MemberRole.Admin)]
    [InlineData(MemberRole.Member)]
    public void Create_WithDifferentRoles_ShouldSucceed(MemberRole role)
    {
        // Act
        var member = GangMember.Create(_gangId, _userId, role, _userId);

        // Assert
        member.Role.Should().Be(role);
    }

    [Fact]
    public void Create_ShouldRaiseMemberJoinedEvent()
    {
        // Act
        var member = GangMember.Create(_gangId, _userId, MemberRole.Member, _userId);

        // Assert
        member.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<MemberJoinedEvent>()
            .Which.Should().Match<MemberJoinedEvent>(e =>
                e.GangId == _gangId &&
                e.UserId == _userId &&
                e.MemberId == member.Id);
    }

    [Fact]
    public void SetRole_ShouldUpdateRole()
    {
        // Arrange
        var member = GangMember.Create(_gangId, _userId, MemberRole.Member, _userId);
        var updater = Guid.NewGuid();

        // Act
        member.SetRole(MemberRole.Admin, updater);

        // Assert
        member.Role.Should().Be(MemberRole.Admin);
        member.UpdatedBy.Should().Be(updater);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var member = GangMember.Create(_gangId, _userId, MemberRole.Member, _userId);
        var updater = Guid.NewGuid();

        // Act
        member.Deactivate(updater);

        // Assert
        member.IsActive.Should().BeFalse();
        member.UpdatedBy.Should().Be(updater);
    }

    [Fact]
    public void Reactivate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var member = GangMember.Create(_gangId, _userId, MemberRole.Member, _userId);
        member.Deactivate(Guid.NewGuid());
        var reactivator = Guid.NewGuid();

        // Act
        member.Reactivate(reactivator);

        // Assert
        member.IsActive.Should().BeTrue();
        member.UpdatedBy.Should().Be(reactivator);
    }
}
