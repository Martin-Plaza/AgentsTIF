using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Domain.Enums;

namespace ServiControl.Application.Services;

public class MetricaService : IMetricaService
{
    private readonly IMetricaRepository _metricaRepository;
    private readonly ITrabajoRepository _trabajoRepository;
    private readonly ICostoRepository _costoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public MetricaService(
        IMetricaRepository metricaRepository,
        ITrabajoRepository trabajoRepository,
        ICostoRepository costoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _metricaRepository = metricaRepository;
        _trabajoRepository = trabajoRepository;
        _costoRepository = costoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<MetricaResponse> GenerarPorRangoAsync(
        GenerateMetricaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _usuarioRepository.ExistsByIdAsync(request.UsuarioId, cancellationToken))
        {
            throw new ArgumentException("El usuario indicado no existe.", nameof(request.UsuarioId));
        }

        var trabajos = await _trabajoRepository.GetByUsuarioAndFechaRangeAsync(
            request.UsuarioId,
            request.PeriodoInicio,
            request.PeriodoFin,
            cancellationToken);

        var trabajosFinalizados = trabajos
            .Where(trabajo => trabajo.Estado == EstadoTrabajo.Finalizado)
            .ToList();

        var costos = await _costoRepository.GetByTrabajoIdsAsync(
            trabajosFinalizados.Select(trabajo => trabajo.Id),
            cancellationToken);

        var montoTotalPeriodo = costos
            .Where(costo => costo.CostoFinal.HasValue)
            .Sum(costo => costo.CostoFinal!.Value);

        var trabajosPendientes = trabajos.Count(trabajo => trabajo.Estado == EstadoTrabajo.Pendiente);

        var metrica = new Metrica(
            request.UsuarioId,
            montoTotalPeriodo,
            trabajosFinalizados.Count,
            trabajosPendientes,
            request.PeriodoInicio,
            request.PeriodoFin);

        var created = await _metricaRepository.AddAsync(metrica, cancellationToken);

        return MapToResponse(created);
    }

    private static MetricaResponse MapToResponse(Metrica metrica)
    {
        return new MetricaResponse(
            metrica.Id,
            metrica.UsuarioId,
            metrica.MontoTotalPeriodo,
            metrica.CantidadTrabajos,
            metrica.TrabajosPendientes,
            metrica.MontoPromedioTrabajo,
            metrica.PeriodoInicio,
            metrica.PeriodoFin);
    }
}
