using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface ITrabajoRepository
{
    Task<Trabajo> AddAsync(Trabajo trabajo, CancellationToken cancellationToken = default);
    Task UpdateAsync(Trabajo trabajo, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trabajo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trabajo>> GetPendientesAsync(CancellationToken cancellationToken = default);
    Task<Trabajo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trabajo>> GetByUsuarioAndFechaRangeAsync(
        int usuarioId,
        DateTime periodoInicio,
        DateTime periodoFin,
        CancellationToken cancellationToken = default);
}
