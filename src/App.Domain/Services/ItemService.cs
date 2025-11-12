using App.Domain.Abstractions;
using App.Domain.Entities;
using App.Domain.Errors;
using App.Domain.Models;

namespace App.Domain.Services;

public sealed class ItemService : IItemService
{
    private const int MaxPageSize = 100;
    private const string NameField = "Name";
    private const string PriceField = "Price";

    private readonly IItemRepository _repository;

    public ItemService(IItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<DomainResult<Item>> CreateAsync(CreateItemModel model, CancellationToken cancellationToken)
    {
        var validationResult = ValidateItem(model.Name, model.Price);
        if (validationResult is not null)
        {
            return DomainResult<Item>.Failure(validationResult);
        }

        var normalizedName = NormalizeName(model.Name);

        var exists = await _repository.ExistsByNameAsync(normalizedName, null, cancellationToken).ConfigureAwait(false);
        if (exists)
        {
            return DomainResult<Item>.Failure(new ConflictError("Элемент с таким именем уже существует."));
        }

        var item = new Item(Guid.NewGuid(), normalizedName, model.Price, DateTime.UtcNow);
        await _repository.AddAsync(item, cancellationToken).ConfigureAwait(false);

        return DomainResult<Item>.Success(item);
    }

    public async Task<DomainResult<Item>> UpdateAsync(Guid id, UpdateItemModel model, CancellationToken cancellationToken)
    {
        var validationResult = ValidateItem(model.Name, model.Price);
        if (validationResult is not null)
        {
            return DomainResult<Item>.Failure(validationResult);
        }

        var item = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (item is null)
        {
            return DomainResult<Item>.Failure(new NotFoundError(id));
        }

        var normalizedName = NormalizeName(model.Name);
        var exists = await _repository.ExistsByNameAsync(normalizedName, id, cancellationToken).ConfigureAwait(false);
        if (exists)
        {
            return DomainResult<Item>.Failure(new ConflictError("Элемент с таким именем уже существует."));
        }

        item.Update(normalizedName, model.Price);
        await _repository.UpdateAsync(item, cancellationToken).ConfigureAwait(false);

        return DomainResult<Item>.Success(item);
    }

    public async Task<DomainResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (item is null)
        {
            return DomainResult.Failure(new NotFoundError(id));
        }

        await _repository.DeleteAsync(item, cancellationToken).ConfigureAwait(false);
        return DomainResult.Success();
    }

    public async Task<DomainResult<Item>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (item is null)
        {
            return DomainResult<Item>.Failure(new NotFoundError(id));
        }

        return DomainResult<Item>.Success(item);
    }

    public async Task<DomainResult<IReadOnlyList<Item>>> ListAsync(int skip, int take, CancellationToken cancellationToken)
    {
        var issues = new List<ValidationIssue>();
        if (skip < 0)
        {
            issues.Add(new ValidationIssue(nameof(skip), "Пропуск должен быть больше или равен 0."));
        }

        if (take <= 0 || take > MaxPageSize)
        {
            issues.Add(new ValidationIssue(nameof(take), $"Значение должен быть между 1 и {MaxPageSize}."));
        }

        if (issues.Count > 0)
        {
            return DomainResult<IReadOnlyList<Item>>.Failure(new ValidationFailedError(issues));
        }

        var items = await _repository.ListAsync(skip, take, cancellationToken).ConfigureAwait(false);
        return DomainResult<IReadOnlyList<Item>>.Success(items);
    }

    private static ValidationFailedError? ValidateItem(string name, decimal price)
    {
        var issues = new List<ValidationIssue>();
        if (string.IsNullOrWhiteSpace(name))
        {
            issues.Add(new ValidationIssue(NameField, "Имя обязательно."));
        }
        else if (name.Trim().Length > 100)
        {
            issues.Add(new ValidationIssue(NameField, "Имя должно быть меньше или равно 100 символам."));
        }

        if (price <= 0)
        {
            issues.Add(new ValidationIssue(PriceField, "Цена должна быть больше 0."));
        }

        return issues.Count == 0 ? null : new ValidationFailedError(issues);
    }

    private static string NormalizeName(string name) => name.Trim();
}
