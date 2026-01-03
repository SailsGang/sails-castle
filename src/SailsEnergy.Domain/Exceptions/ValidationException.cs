namespace SailsEnergy.Domain.Exceptions;

public sealed class ValidationException : DomainException
{
    private const string _defaultCode = "VALIDATION_ERROR";

    public string? PropertyName { get; }

    public ValidationException() : base(_defaultCode, "Validation failed.")
    {
    }

    public ValidationException(string message) : base(_defaultCode, message)
    {
    }

    public ValidationException(string message, Exception innerException)
        : base(_defaultCode, message, innerException)
    {
    }

    public ValidationException(string propertyName, string message)
        : base(_defaultCode, message)
    {
        PropertyName = propertyName;
    }

    public static void ThrowIfNullOrWhiteSpace(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException(propertyName, $"{propertyName} is required.");
        }
    }
}
