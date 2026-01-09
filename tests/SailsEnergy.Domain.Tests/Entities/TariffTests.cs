namespace SailsEnergy.Domain.Tests.Entities;

using SailsEnergy.Domain.Entities;
using Events;
using Exceptions;

public class TariffTests
{
    private readonly Guid _gangId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var tariff = Tariff.Create(_gangId, 2.50m, "uah", _userId);

        // Assert
        tariff.GangId.Should().Be(_gangId);
        tariff.PricePerKwh.Should().Be(2.50m);
        tariff.Currency.Should().Be("UAH"); // Should be uppercase
        tariff.SetByUserId.Should().Be(_userId);
        tariff.EffectiveFrom.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        tariff.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldRaiseTariffChangedEvent()
    {
        // Act
        var tariff = Tariff.Create(_gangId, 2.50m, "UAH", _userId);

        // Assert
        tariff.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TariffChangedEvent>()
            .Which.Should().Match<TariffChangedEvent>(e =>
                e.GangId == _gangId &&
                e.TariffId == tariff.Id &&
                e.PricePerKwh == 2.50m);
    }

    [Fact]
    public void Create_WithZeroPrice_ShouldSucceed()
    {
        // Act (zero price = free charging)
        var tariff = Tariff.Create(_gangId, 0m, "UAH", _userId);

        // Assert
        tariff.PricePerKwh.Should().Be(0m);
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrow()
    {
        // Act & Assert
        var act = () => Tariff.Create(_gangId, -1m, "UAH", _userId);
        act.Should().Throw<ValidationException>()
           .Where(e => e.PropertyName == "pricePerKwh");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidCurrency_ShouldThrow(string? currency)
    {
        // Act & Assert
        var act = () => Tariff.Create(_gangId, 2.50m, currency!, _userId);
        act.Should().Throw<ValidationException>()
           .Where(e => e.PropertyName == "currency");
    }

    [Theory]
    [InlineData("uah", "UAH")]
    [InlineData("Usd", "USD")]
    [InlineData("EUR", "EUR")]
    public void Create_ShouldNormalizeCurrencyToUppercase(string input, string expected)
    {
        // Act
        var tariff = Tariff.Create(_gangId, 1m, input, _userId);

        // Assert
        tariff.Currency.Should().Be(expected);
    }
}
