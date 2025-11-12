using App.Domain.Entities;
using App.Domain.Errors;
using App.Domain.Models;

namespace App.Domain.Services;

public interface IItemService
{
    Task<DomainResult<Item>> CreateAsync(CreateItemModel model, CancellationToken cancellationToken);

    Task<DomainResult<Item>> UpdateAsync(Guid id, UpdateItemModel model, CancellationToken cancellationToken);

    Task<DomainResult> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<DomainResult<Item>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<DomainResult<IReadOnlyList<Item>>> ListAsync(int skip, int take, CancellationToken cancellationToken);
}
