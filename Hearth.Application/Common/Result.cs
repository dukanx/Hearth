namespace Hearth.Application.Common;

public enum ErrorType
{
    Validation,
    Unauthorized,
    Forbidden,
    Conflict,
    NotFound,
    Failure
}

public sealed record Error(ErrorType Type, string Code, string Message)
{
    public static Error Validation(string message, string code = "Validation") =>
        new(ErrorType.Validation, code, message);

    public static Error Unauthorized(string message, string code = "Unauthorized") =>
        new(ErrorType.Unauthorized, code, message);

    public static Error Forbidden(string message, string code = "Forbidden") =>
        new(ErrorType.Forbidden, code, message);

    public static Error Conflict(string message, string code = "Conflict") =>
        new(ErrorType.Conflict, code, message);

    public static Error NotFound(string message, string code = "NotFound") =>
        new(ErrorType.NotFound, code, message);
}

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result<T> Failure<T>(Error error) => new(default, false, error);
}

public class Result<T> : Result
{
    private readonly T? _value;

    protected internal Result(T? value, bool isSuccess, Error? error) : base(isSuccess, error)
        => _value = value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Neuspešan Result nema vrednost.");

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure<T>(error);
}
