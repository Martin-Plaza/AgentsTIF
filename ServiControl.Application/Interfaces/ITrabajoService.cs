using ServiControl.Application.DTOs;

namespace ServiControl.Application.Interfaces;

public interface ITrabajoService
{
    Task<TrabajoResponse> CrearAsync(CreateTrabajoRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrabajoResponse>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrabajoResponse>> ObtenerPendientesAsync(CancellationToken cancellationToken = default);
    Task<TrabajoResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TrabajoResponse> CambiarEstadoAsync(
        int trabajoId,
        UpdateTrabajoStatusRequest request,
        CancellationToken cancellationToken = default);
}
