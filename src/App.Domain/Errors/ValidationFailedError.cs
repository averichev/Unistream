namespace App.Domain.Errors;

public sealed record ValidationFailedError(IReadOnlyCollection<ValidationIssue> Issues)
    : DomainError("validation_failed", "Данные не валидны");
