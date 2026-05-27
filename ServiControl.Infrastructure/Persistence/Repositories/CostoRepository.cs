using Microsoft.EntityFrameworkCore;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class CostoRepository : ICostoRepository
{
    private readonly ApplicationDbContext _context;

    public CostoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Costo> AddAsync(Costo costo, CancellationToken cancellationToken = default)
    {
        await _context.Costos.AddAsync(costo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return costo;
    }

    public async Task UpdateAsync(Costo costo, CancellationToken cancellationToken = default)
    {
        _context.Costos.Update(costo);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Costo?> GetByTrabajoIdAsync(int trabajoId, CancellationToken cancellationToken = default)
    {
        return await _context.Costos
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

        return await _context.Costos
            .AsNoTracking()
            .Where(costo => ids.Contains(costo.TrabajoId))
            .ToListAsync(cancellationToken);
    }
}
