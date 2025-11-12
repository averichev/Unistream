namespace App.Domain.Errors;

public sealed record NotFoundError(Guid Id)
    : DomainError("not_found", $"Запись '{Id}' не найдена.");
