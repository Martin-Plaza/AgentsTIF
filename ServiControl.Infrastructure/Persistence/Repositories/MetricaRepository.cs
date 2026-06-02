using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class MetricaRepository : GenericRepository<Metrica>, IMetricaRepository
{
    public MetricaRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
