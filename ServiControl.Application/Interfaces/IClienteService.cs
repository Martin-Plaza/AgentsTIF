using ServiControl.Application.DTOs;

namespace ServiControl.Application.Interfaces;

public interface IClienteService
{
    Task<ClienteResponse> CrearAsync(CreateClienteRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClienteResponse>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<ClienteResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ClienteResponse> ActualizarAsync(
        int id,
        UpdateClienteRequest request,
        CancellationToken cancellationToken = default);
}
