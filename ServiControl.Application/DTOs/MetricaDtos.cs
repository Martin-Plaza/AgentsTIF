namespace ServiControl.Application.DTOs;

public record GenerateMetricaRequest(
    int UsuarioId,
    DateTime PeriodoInicio,
    DateTime PeriodoFin);

public record MetricaResponse(
    int Id,
    int UsuarioId,
    decimal MontoTotalPeriodo,
    int CantidadTrabajos,
    int TrabajosPendientes,
    decimal MontoPromedioTrabajo,
    DateTime PeriodoInicio,
    DateTime PeriodoFin);
