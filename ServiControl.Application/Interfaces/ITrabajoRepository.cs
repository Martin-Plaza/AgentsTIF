using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface ITrabajoRepository : IGenericRepository<Trabajo>
{
    Task<IReadOnlyList<Trabajo>> GetPendientesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trabajo>> GetByUsuarioAndFechaRangeAsync(
        int usuarioId,
        DateTime periodoInicio,
        DateTime periodoFin,
        CancellationToken cancellationToken = default);
}
