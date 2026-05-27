using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface IMetricaRepository
{
    Task<Metrica> AddAsync(Metrica metrica, CancellationToken cancellationToken = default);
}
