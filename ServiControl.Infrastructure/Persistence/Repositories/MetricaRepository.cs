using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class MetricaRepository : IMetricaRepository
{
    private readonly ApplicationDbContext _context;

    public MetricaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Metrica> AddAsync(Metrica metrica, CancellationToken cancellationToken = default)
    {
        await _context.Metricas.AddAsync(metrica, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return metrica;
    }
}
