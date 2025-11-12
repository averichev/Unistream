using App.Domain.Errors;

namespace App.Domain.Services;

public sealed class DomainResult
{
    private DomainResult(bool isSuccess, DomainError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public DomainError? Error { get; }

    public static DomainResult Success() => new(true, null);

    public static DomainResult Failure(DomainError error) => new(false, error);
}

public sealed class DomainResult<T>
{
    private DomainResult(bool isSuccess, T? value, DomainError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }

    public T? Value { get; }

    public DomainError? Error { get; }

    public static DomainResult<T> Success(T value) => new(true, value, null);

    public static DomainResult<T> Failure(DomainError error) => new(false, default, error);
}
