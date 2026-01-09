using FluentValidation.TestHelper;
using SailsEnergy.Application.Features.Auth.Commands.Register;

namespace SailsEnergy.Application.Tests.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new RegisterCommand(
            "test@example.com",
            "Password123",
            "Password123",
            "TestUser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyEmail_ShouldFail(string? email)
    {
        var command = new RegisterCommand(
            email!,
            "Password123",
            "Password123",
            "TestUser",
            null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldFail()
    {
        var command = new RegisterCommand(
            "not-an-email",
            "Password123",
            "Password123",
            "TestUser",
            null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    public void Validate_WithShortPassword_ShouldFail(string password)
    {
        var command = new RegisterCommand(
            "test@example.com",
            password,
            password,
            "TestUser",
            null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters.");
    }

    [Fact]
    public void Validate_WithNoUppercase_ShouldFail()
    {
        var command = new RegisterCommand(
            "test@example.com",
            "password123",
            "password123",
            "TestUser",
            null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Validate_WithNoLowercase_ShouldFail()
    {
        var command = new RegisterCommand(
            "test@example.com",
            "PASSWORD123",
            "PASSWORD123",
            "TestUser",
            null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Validate_WithNoDigit_ShouldFail()
    {
        var command = new RegisterCommand(
            "test@example.com",
            "PasswordABC",
            "PasswordABC",
            "TestUser",
            null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one digit.");
    }

    [Fact]
    public void Validate_WithMismatchedPasswords_ShouldFail()
    {
        var command = new RegisterCommand(
            "test@example.com",
            "Password123",
            "DifferentPassword123",
            "TestUser",
            null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
            .WithErrorMessage("Passwords do not match.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyDisplayName_ShouldFail(string? displayName)
    {
        var command = new RegisterCommand(
            "test@example.com",
            "Password123",
            "Password123",
            displayName!,
            null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Validate_WithTooLongDisplayName_ShouldFail()
    {
        var command = new RegisterCommand(
            "test@example.com",
            "Password123",
            "Password123",
            new string('a', 51),
            null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DisplayName)
            .WithErrorMessage("Display name cannot exceed 50 characters.");
    }
}
