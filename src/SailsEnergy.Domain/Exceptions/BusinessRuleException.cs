namespace SailsEnergy.Domain.Exceptions;

public sealed class BusinessRuleException : DomainException
{
    private const string _defaultCode = "BUSINESS_RULE_VIOLATION";

    public BusinessRuleException() : base(_defaultCode, "Business rule violation.")
    {
    }

    public BusinessRuleException(string message) : base(_defaultCode, message)
    {
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(_defaultCode, message, innerException)
    {
    }

    public BusinessRuleException(string code, string message)
        : base(code, message)
    {
    }
}
