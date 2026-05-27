using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface IClienteRepository
{
    Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
}
