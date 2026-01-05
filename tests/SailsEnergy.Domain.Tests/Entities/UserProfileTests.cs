namespace SailsEnergy.Domain.Tests.Entities;

using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;

public class UserProfileTests
{
    private readonly Guid _identityId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var profile = UserProfile.Create(_identityId, "JohnDoe", _identityId);

        // Assert
        profile.IdentityId.Should().Be(_identityId);
        profile.DisplayName.Should().Be("JohnDoe");
        profile.FirstName.Should().BeNull();
        profile.LastName.Should().BeNull();
        profile.AvatarUrl.Should().BeNull();
        profile.Id.Should().NotBeEmpty();
        profile.CreatedBy.Should().Be(_identityId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidDisplayName_ShouldThrow(string? displayName)
    {
        // Act & Assert
        var act = () => UserProfile.Create(_identityId, displayName!, _identityId);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetDisplayName_WithValidName_ShouldUpdate()
    {
        // Arrange
        var profile = UserProfile.Create(_identityId, "Original", _identityId);
        var updater = Guid.NewGuid();

        // Act
        profile.SetDisplayName("Updated", updater);

        // Assert
        profile.DisplayName.Should().Be("Updated");
        profile.UpdatedBy.Should().Be(updater);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetDisplayName_WithInvalidName_ShouldThrow(string? displayName)
    {
        // Arrange
        var profile = UserProfile.Create(_identityId, "Original", _identityId);

        // Act & Assert
        var act = () => profile.SetDisplayName(displayName!, Guid.NewGuid());
        act.Should().Throw<ValidationException>()
           .Where(e => e.PropertyName == "displayName");
    }

    [Fact]
    public void SetName_ShouldUpdateBothNames()
    {
        // Arrange
        var profile = UserProfile.Create(_identityId, "JohnDoe", _identityId);
        var updater = Guid.NewGuid();

        // Act
        profile.SetName("John", "Doe", updater);

        // Assert
        profile.FirstName.Should().Be("John");
        profile.LastName.Should().Be("Doe");
        profile.UpdatedBy.Should().Be(updater);
    }

    [Fact]
    public void SetName_WithNulls_ShouldClear()
    {
        // Arrange
        var profile = UserProfile.Create(_identityId, "JohnDoe", _identityId);
        profile.SetName("John", "Doe", Guid.NewGuid());

        // Act
        profile.SetName(null, null, Guid.NewGuid());

        // Assert
        profile.FirstName.Should().BeNull();
        profile.LastName.Should().BeNull();
    }

    [Fact]
    public void SetAvatar_ShouldUpdate()
    {
        // Arrange
        var profile = UserProfile.Create(_identityId, "JohnDoe", _identityId);
        var avatarUrl = new Uri("https://example.com/avatar.jpg");

        // Act
        profile.SetAvatar(avatarUrl, Guid.NewGuid());

        // Assert
        profile.AvatarUrl.Should().Be(avatarUrl);
    }

    [Fact]
    public void SetAvatar_ToNull_ShouldClear()
    {
        // Arrange
        var profile = UserProfile.Create(_identityId, "JohnDoe", _identityId);
        profile.SetAvatar(new Uri("https://example.com/avatar.jpg"), Guid.NewGuid());

        // Act
        profile.SetAvatar(null, Guid.NewGuid());

        // Assert
        profile.AvatarUrl.Should().BeNull();
    }
}
