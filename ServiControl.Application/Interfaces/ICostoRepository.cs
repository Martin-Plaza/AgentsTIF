using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface ICostoRepository : IGenericRepository<Costo>
{
    Task<Costo?> GetByTrabajoIdAsync(int trabajoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Costo>> GetByTrabajoIdsAsync(
        IEnumerable<int> trabajoIds,
        CancellationToken cancellationToken = default);
}
