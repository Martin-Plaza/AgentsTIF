using Microsoft.EntityFrameworkCore;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class CostoRepository : GenericRepository<Costo>, ICostoRepository
{
    public CostoRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Costo?> GetByTrabajoIdAsync(int trabajoId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(costo => costo.TrabajoId == trabajoId, cancellationToken);
    }

    public async Task<IReadOnlyList<Costo>> GetByTrabajoIdsAsync(
        IEnumerable<int> trabajoIds,
        CancellationToken cancellationToken = default)
    {
        var ids = trabajoIds.ToList();

        if (ids.Count == 0)
        {
            return [];
        }

        return await DbSet
            .AsNoTracking()
            .Where(costo => ids.Contains(costo.TrabajoId))
            .ToListAsync(cancellationToken);
    }
}
