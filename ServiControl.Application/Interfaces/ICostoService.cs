using ServiControl.Application.DTOs;

namespace ServiControl.Application.Interfaces;

public interface ICostoService
{
    Task<CostoResponse> RegistrarCostoEstimadoAsync(
        RegisterCostoEstimadoRequest request,
        CancellationToken cancellationToken = default);

    Task<CostoResponse> RegistrarCostoFinalAsync(
        RegisterCostoFinalRequest request,
        CancellationToken cancellationToken = default);
}
