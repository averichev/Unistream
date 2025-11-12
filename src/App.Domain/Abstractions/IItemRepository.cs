using App.Domain.Entities;

namespace App.Domain.Abstractions;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Item>> ListAsync(int skip, int take, CancellationToken cancellationToken);

    Task AddAsync(Item item, CancellationToken cancellationToken);

    Task UpdateAsync(Item item, CancellationToken cancellationToken);

    Task DeleteAsync(Item item, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string name, Guid? excludingId, CancellationToken cancellationToken);
}
