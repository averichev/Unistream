using App.Domain.Abstractions;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Data.Repositories;

public sealed class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;

    public ItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Item item, CancellationToken cancellationToken)
    {
        await _context.Items.AddAsync(item, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Item item, CancellationToken cancellationToken)
    {
        _context.Items.Remove(item);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludingId, CancellationToken cancellationToken)
    {
        return await _context.Items
            .AnyAsync(x => x.Name == name && (!excludingId.HasValue || x.Id != excludingId), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Items.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Item>> ListAsync(int skip, int take, CancellationToken cancellationToken)
    {
        return await _context.Items
            .OrderBy(x => x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task UpdateAsync(Item item, CancellationToken cancellationToken)
    {
        _context.Items.Update(item);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
