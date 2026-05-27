namespace ServiControl.Application.DTOs;

public record RegisterCostoEstimadoRequest(
    int TrabajoId,
    decimal CostoEstimado);

public record RegisterCostoFinalRequest(
    int TrabajoId,
    decimal CostoFinal);

public record CostoResponse(
    int Id,
    int TrabajoId,
    decimal? CostoEstimado,
    decimal? CostoFinal);
