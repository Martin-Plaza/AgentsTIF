using Microsoft.EntityFrameworkCore;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Domain.Enums;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class TrabajoRepository : ITrabajoRepository
{
    private readonly ApplicationDbContext _context;

    public TrabajoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Trabajo> AddAsync(Trabajo trabajo, CancellationToken cancellationToken = default)
    {
        await _context.Trabajos.AddAsync(trabajo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return trabajo;
    }

    public async Task UpdateAsync(Trabajo trabajo, CancellationToken cancellationToken = default)
    {
        _context.Trabajos.Update(trabajo);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Trabajo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Trabajos
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Trabajo>> GetPendientesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Trabajos
            .AsNoTracking()
            .Where(trabajo => trabajo.Estado == EstadoTrabajo.Pendiente)
            .ToListAsync(cancellationToken);
    }

    public async Task<Trabajo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Trabajos
            .FirstOrDefaultAsync(trabajo => trabajo.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Trabajo>> GetByUsuarioAndFechaRangeAsync(
        int usuarioId,
        DateTime periodoInicio,
        DateTime periodoFin,
        CancellationToken cancellationToken = default)
    {
        return await _context.Trabajos
            .AsNoTracking()
            .Where(trabajo =>
                trabajo.UsuarioId == usuarioId &&
                trabajo.Fecha >= periodoInicio &&
                trabajo.Fecha <= periodoFin)
            .ToListAsync(cancellationToken);
    }
}
