using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using SailsEnergy.Infrastructure.Identity;

namespace SailsEnergy.Application.Tests.Services;

public class JwtServiceTests
{
    private readonly JwtService _sut;
    private readonly JwtSettings _settings;

    public JwtServiceTests()
    {
        _settings = new JwtSettings
        {
            Secret = "this-is-a-very-secret-key-for-testing-purposes-123!",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        };
        _sut = new JwtService(Options.Create(_settings));
    }

    #region GenerateAccessToken Tests

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidJwt()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var roles = new[] { "User", "Admin" };

        // Act
        var token = _sut.GenerateAccessToken(userId, email, roles);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        
        jwt.Should().NotBeNull();
        jwt.Issuer.Should().Be(_settings.Issuer);
        jwt.Audiences.Should().Contain(_settings.Audience);
    }

    [Fact]
    public void GenerateAccessToken_ShouldContainUserClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var roles = new[] { "User" };

        // Act
        var token = _sut.GenerateAccessToken(userId, email, roles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        
        jwt.Claims.Should().Contain(c => 
            c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jwt.Claims.Should().Contain(c => 
            c.Type == ClaimTypes.Email && c.Value == email);
        jwt.Claims.Should().Contain(c => 
            c.Type == ClaimTypes.Role && c.Value == "User");
    }

    [Fact]
    public void GenerateAccessToken_ShouldContainMultipleRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "admin@example.com";
        var roles = new[] { "User", "Admin", "SuperAdmin" };

        // Act
        var token = _sut.GenerateAccessToken(userId, email, roles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        
        var roleClaims = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        roleClaims.Should().HaveCount(3);
        roleClaims.Select(c => c.Value).Should().BeEquivalentTo(roles);
    }

    [Fact]
    public void GenerateAccessToken_ShouldHaveCorrectExpiration()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _sut.GenerateAccessToken(userId, "test@example.com", []);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        
        var expectedExpiry = beforeGeneration.AddMinutes(_settings.AccessTokenExpirationMinutes);
        jwt.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromSeconds(5));
    }

    #endregion

    #region GenerateRefreshToken Tests

    [Fact]
    public void GenerateRefreshToken_ShouldReturnBase64String()
    {
        // Act
        var token = _sut.GenerateRefreshToken();

        // Assert
        token.Should().NotBeNullOrEmpty();
        var act = () => Convert.FromBase64String(token);
        act.Should().NotThrow();
    }

    [Fact]
    public void GenerateRefreshToken_ShouldBeUnique()
    {
        // Act
        var token1 = _sut.GenerateRefreshToken();
        var token2 = _sut.GenerateRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldHaveSufficientLength()
    {
        // Act
        var token = _sut.GenerateRefreshToken();

        // Assert
        var bytes = Convert.FromBase64String(token);
        bytes.Should().HaveCount(64); // 512 bits
    }

    #endregion

    #region ValidateRefreshToken Tests

    [Fact]
    public void ValidateRefreshToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var token = _sut.GenerateRefreshToken();

        // Act
        var result = _sut.ValidateRefreshToken(token);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void ValidateRefreshToken_WithEmptyToken_ShouldReturnFalse(string? token)
    {
        // Act
        var result = _sut.ValidateRefreshToken(token!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateRefreshToken_WithInvalidBase64_ShouldReturnFalse()
    {
        // Act
        var result = _sut.ValidateRefreshToken("not-valid-base64!!!");

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}
