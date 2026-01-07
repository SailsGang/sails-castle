namespace SailsEnergy.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }

    protected Result(bool isSuccess, string? errorCode = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new(true);
    public static Result Failure(string code, string message) => new(false, code, message);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T value) : base(true) => Value = value;

    protected Result(string code, string message) : base(false, code, message) { }
    public static Result<T> Ok(T value) => new ResultSuccess<T>(value);
    public static new Result<T> Fail(string code, string message) => new ResultFailure<T>(code, message);
}
internal class ResultSuccess<T>(T value) : Result<T>(value);

internal class ResultFailure<T>(string code, string message) : Result<T>(code, message);
