namespace Temp.Services.Results;

/// <summary>
/// Represents the result of an operation
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public string? ErrorCode { get; }
    public Dictionary<string, string[]>? ValidationErrors { get; }

    protected Result(bool isSuccess, string? error, string? errorCode = null, Dictionary<string, string[]>? validationErrors = null)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException("Successful result cannot have an error");
        if (!isSuccess && error == null)
            throw new InvalidOperationException("Failed result must have an error");

        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
        ValidationErrors = validationErrors;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);
    public static Result ValidationFailure(string error, Dictionary<string, string[]> validationErrors)
        => new(false, error, "VALIDATION_ERROR", validationErrors);

    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result<T> Failure<T>(string error, string? errorCode = null) => new(default!, false, error, errorCode);
    public static Result<T> ValidationFailure<T>(string error, Dictionary<string, string[]> validationErrors)
        => new(default!, false, error, "VALIDATION_ERROR", validationErrors);
}

/// <summary>
/// Represents the result of an operation with a return value
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }

    protected internal Result(T? value, bool isSuccess, string? error, string? errorCode = null, Dictionary<string, string[]>? validationErrors = null)
        : base(isSuccess, error, errorCode, validationErrors)
    {
        Value = value;
    }
}
