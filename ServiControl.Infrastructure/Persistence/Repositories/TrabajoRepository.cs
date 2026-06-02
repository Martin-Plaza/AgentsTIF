using Microsoft.EntityFrameworkCore;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Domain.Enums;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class TrabajoRepository : GenericRepository<Trabajo>, ITrabajoRepository
{
    public TrabajoRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Trabajo>> GetPendientesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(trabajo => trabajo.Estado == EstadoTrabajo.Pendiente)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Trabajo>> GetByUsuarioAndFechaRangeAsync(
        int usuarioId,
        DateTime periodoInicio,
        DateTime periodoFin,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(trabajo =>
                trabajo.UsuarioId == usuarioId &&
                trabajo.Fecha >= periodoInicio &&
                trabajo.Fecha <= periodoFin)
            .ToListAsync(cancellationToken);
    }
}
