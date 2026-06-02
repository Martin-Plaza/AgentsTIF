using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Domain.Enums;

namespace ServiControl.Application.Services;

// Modulo: Trabajos
// Capa: Application
// Responsabilidad: Coordina trabajos y delega reglas de estado a la entidad de dominio.
public class TrabajoService : ITrabajoService
{
    private readonly ITrabajoRepository _trabajoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public TrabajoService(
        ITrabajoRepository trabajoRepository,
        IClienteRepository clienteRepository,
        IUsuarioRepository usuarioRepository)
    {
        _trabajoRepository = trabajoRepository;
        _clienteRepository = clienteRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<TrabajoResponse> CrearAsync(
        CreateTrabajoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _clienteRepository.ExistsByIdAsync(request.ClienteId, cancellationToken))
        {
            throw new ArgumentException("El cliente indicado no existe.", nameof(request.ClienteId));
        }

        if (!await _usuarioRepository.ExistsByIdAsync(request.UsuarioId, cancellationToken))
        {
            throw new ArgumentException("El usuario indicado no existe.", nameof(request.UsuarioId));
        }

        var trabajo = new Trabajo(
            request.ClienteId,
            request.UsuarioId,
            request.CategoriaServicio,
            request.Descripcion,
            request.Fecha,
            request.Direccion,
            request.Observaciones);

        var created = await _trabajoRepository.AddAsync(trabajo, cancellationToken);

        return MapToResponse(created);
    }

    public async Task<IReadOnlyList<TrabajoResponse>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var trabajos = await _trabajoRepository.GetAllAsync(cancellationToken);

        return trabajos.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<TrabajoResponse>> ObtenerPendientesAsync(
        CancellationToken cancellationToken = default)
    {
        var trabajos = await _trabajoRepository.GetPendientesAsync(cancellationToken);

        return trabajos.Select(MapToResponse).ToList();
    }

    public async Task<TrabajoResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var trabajo = await _trabajoRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ArgumentException("El trabajo indicado no existe.", nameof(id));

        return MapToResponse(trabajo);
    }

    public async Task<TrabajoResponse> CambiarEstadoAsync(
        int trabajoId,
        UpdateTrabajoStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var trabajo = await _trabajoRepository.GetByIdAsync(trabajoId, cancellationToken)
            ?? throw new ArgumentException("El trabajo indicado no existe.", nameof(trabajoId));

        switch (request.Estado)
        {
            case EstadoTrabajo.EnProceso:
                trabajo.Iniciar();
                break;
            case EstadoTrabajo.Finalizado:
                trabajo.Finalizar();
                break;
            case EstadoTrabajo.Cancelado:
                trabajo.Cancelar();
                break;
            case EstadoTrabajo.Pendiente:
                throw new InvalidOperationException("No se puede volver un trabajo a estado pendiente.");
            default:
                throw new ArgumentException("El estado indicado no es valido.", nameof(request.Estado));
        }

        await _trabajoRepository.UpdateAsync(trabajo, cancellationToken);

        return MapToResponse(trabajo);
    }

    private static TrabajoResponse MapToResponse(Trabajo trabajo)
    {
        return new TrabajoResponse(
            trabajo.Id,
            trabajo.ClienteId,
            trabajo.UsuarioId,
            trabajo.CategoriaServicio,
            trabajo.Descripcion,
            trabajo.Fecha,
            trabajo.Direccion,
            trabajo.Observaciones,
            trabajo.Estado);
    }
}
