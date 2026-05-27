using ServiControl.Application.DTOs;

namespace ServiControl.Application.Interfaces;

public interface IMetricaService
{
    Task<MetricaResponse> GenerarPorRangoAsync(
        GenerateMetricaRequest request,
        CancellationToken cancellationToken = default);
}
