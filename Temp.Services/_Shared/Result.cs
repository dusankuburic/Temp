namespace Temp.Services._Shared;


public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly string? _error;
    private readonly string? _errorCode;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot access Value on a failed result. Error: {_error}");

    public string Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("Cannot access Error on a successful result.");

    public string? ErrorCode => _errorCode;

    private Result(T value) {
        IsSuccess = true;
        _value = value;
        _error = null;
        _errorCode = null;
    }

    private Result(string error, string? errorCode = null) {
        IsSuccess = false;
        _value = default;
        _error = error;
        _errorCode = errorCode;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(string error, string? errorCode = null) => new(error, errorCode);

    public static implicit operator Result<T>(T value) => Success(value);

    public Result<TNew> Map<TNew>(Func<T, TNew> mapper) =>
        IsSuccess ? Result<TNew>.Success(mapper(_value!)) : Result<TNew>.Failure(_error!, _errorCode);

    public Result<T> OnSuccess(Action<T> action) {
        if (IsSuccess)
            action(_value!);
        return this;
    }

    public Result<T> OnFailure(Action<string> action) {
        if (IsFailure)
            action(_error!);
        return this;
    }


    public T GetValueOrDefault(T defaultValue = default!) =>
        IsSuccess ? _value! : defaultValue;

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(_error!);
}




public readonly struct Result
{
    private readonly string? _error;
    private readonly string? _errorCode;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public string Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("Cannot access Error on a successful result.");

    public string? ErrorCode => _errorCode;

    private Result(bool isSuccess, string? error = null, string? errorCode = null) {
        IsSuccess = isSuccess;
        _error = error;
        _errorCode = errorCode;
    }

    public static Result Success() => new(true);

    public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);

    public Result OnSuccess(Action action) {
        if (IsSuccess)
            action();
        return this;
    }

    public Result OnFailure(Action<string> action) {
        if (IsFailure)
            action(_error!);
        return this;
    }

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(_error!);
}

public static class ResultExtensions
{
    public static Result<T> ToResult<T>(this T? value, string errorIfNull) where T : class =>
        value is not null ? Result<T>.Success(value) : Result<T>.Failure(errorIfNull, "NOT_FOUND");

    public static Result<T> ToResult<T>(this T? value, string errorIfNull) where T : struct =>
        value.HasValue ? Result<T>.Success(value.Value) : Result<T>.Failure(errorIfNull, "NOT_FOUND");

    public static Result Combine(params Result[] results) {
        foreach (var result in results) {
            if (result.IsFailure)
                return Result.Failure(result.Error, result.ErrorCode);
        }
        return Result.Success();
    }
}