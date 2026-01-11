using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Tariffs.Commands.SetTariff;

namespace SailsEnergy.Application.Tests.Validators;

public class SetTariffValidatorTests
{
    private readonly SetTariffValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new SetTariffCommand(Guid.NewGuid(), 8.5m, "UAH");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyGangId_ShouldFail()
    {
        var command = new SetTariffCommand(Guid.Empty, 8.5m, "UAH");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GangId);
    }

    [Fact]
    public void Validate_WithNegativePrice_ShouldFail()
    {
        var command = new SetTariffCommand(Guid.NewGuid(), -1m, "UAH");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PricePerKwh);
    }

    [Fact]
    public void Validate_WithZeroPrice_ShouldPass()
    {
        var command = new SetTariffCommand(Guid.NewGuid(), 0m, "UAH");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyCurrency_ShouldFail()
    {
        var command = new SetTariffCommand(Guid.NewGuid(), 8.5m, "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Validate_WithLongCurrency_ShouldFail()
    {
        var command = new SetTariffCommand(Guid.NewGuid(), 8.5m, "VERYLONGCURRENCY");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Theory]
    [InlineData("UAH")]
    [InlineData("USD")]
    [InlineData("EUR")]
    public void Validate_WithValidCurrencies_ShouldPass(string currency)
    {
        var command = new SetTariffCommand(Guid.NewGuid(), 8.5m, currency);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
