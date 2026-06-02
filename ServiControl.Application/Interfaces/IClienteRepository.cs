using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface IClienteRepository : IGenericRepository<Cliente>
{
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
}
