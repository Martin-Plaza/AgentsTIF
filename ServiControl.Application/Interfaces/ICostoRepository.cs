using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface ICostoRepository
{
    Task<Costo> AddAsync(Costo costo, CancellationToken cancellationToken = default);
    Task UpdateAsync(Costo costo, CancellationToken cancellationToken = default);
    Task<Costo?> GetByTrabajoIdAsync(int trabajoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Costo>> GetByTrabajoIdsAsync(
        IEnumerable<int> trabajoIds,
        CancellationToken cancellationToken = default);
}
