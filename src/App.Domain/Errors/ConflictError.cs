namespace App.Domain.Errors;

public sealed record ConflictError(string Message)
    : DomainError("conflict", Message);