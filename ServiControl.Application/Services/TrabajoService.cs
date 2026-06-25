using ServiControl.Application.Authorization;
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
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IOperationalUserResolver _operationalUserResolver;

    public TrabajoService(
        ITrabajoRepository trabajoRepository,
        IClienteRepository clienteRepository,
        ICurrentUserContext currentUserContext,
        IOperationalUserResolver operationalUserResolver)
    {
        _trabajoRepository = trabajoRepository;
        _clienteRepository = clienteRepository;
        _currentUserContext = currentUserContext;
        _operationalUserResolver = operationalUserResolver;
    }

    public async Task<TrabajoResponse> CrearAsync(
        CreateTrabajoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _clienteRepository.ExistsByIdAsync(request.ClienteId, cancellationToken))
        {
            throw new ArgumentException("El cliente indicado no existe.", nameof(request.ClienteId));
        }

        var usuarioOperativoId = await _operationalUserResolver
            .ObtenerUsuarioOperativoIdAsync(cancellationToken);

        var trabajo = new Trabajo(
            request.ClienteId,
            usuarioOperativoId,
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
        var usuarioOperativoId = await _operationalUserResolver
            .ObtenerUsuarioOperativoIdAsync(cancellationToken);

        var trabajos = _currentUserContext.IsAdmin
            ? await _trabajoRepository.GetAllAsync(cancellationToken)
            : await _trabajoRepository.GetByUsuarioAsync(
                usuarioOperativoId,
                cancellationToken);

        return trabajos.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<TrabajoResponse>> ObtenerPendientesAsync(
        CancellationToken cancellationToken = default)
    {
        var usuarioOperativoId = await _operationalUserResolver
            .ObtenerUsuarioOperativoIdAsync(cancellationToken);

        var trabajos = _currentUserContext.IsAdmin
            ? await _trabajoRepository.GetPendientesAsync(cancellationToken)
            : await _trabajoRepository.GetPendientesByUsuarioAsync(
                usuarioOperativoId,
                cancellationToken);

        return trabajos.Select(MapToResponse).ToList();
    }

    public async Task<TrabajoResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var trabajo = await ObtenerTrabajoAccesibleAsync(id, cancellationToken);

        return MapToResponse(trabajo);
    }

    public async Task<TrabajoResponse> CambiarEstadoAsync(
        int trabajoId,
        UpdateTrabajoStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var trabajo = await ObtenerTrabajoAccesibleAsync(trabajoId, cancellationToken);

        ValidarTransicion(trabajo.Estado, request.Estado);

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

    private async Task<Trabajo> ObtenerTrabajoAccesibleAsync(
        int trabajoId,
        CancellationToken cancellationToken)
    {
        var usuarioOperativoId = await _operationalUserResolver
            .ObtenerUsuarioOperativoIdAsync(cancellationToken);

        var trabajo = _currentUserContext.IsAdmin
            ? await _trabajoRepository.GetByIdAsync(trabajoId, cancellationToken)
            : await _trabajoRepository.GetByIdAndUsuarioAsync(
                trabajoId,
                usuarioOperativoId,
                cancellationToken);

        return trabajo
            ?? throw new ArgumentException("El trabajo indicado no existe.", nameof(trabajoId));
    }

    private static void ValidarTransicion(EstadoTrabajo estadoActual, EstadoTrabajo nuevoEstado)
    {
        if (!Enum.IsDefined(nuevoEstado))
        {
            throw new ArgumentException("El estado indicado no es valido.", nameof(nuevoEstado));
        }

        if (nuevoEstado == EstadoTrabajo.Pendiente)
        {
            throw new InvalidOperationException("No se puede volver un trabajo a estado pendiente.");
        }

        if (estadoActual is EstadoTrabajo.Finalizado or EstadoTrabajo.Cancelado)
        {
            throw new InvalidOperationException(
                $"No se puede cambiar un trabajo {estadoActual} a {nuevoEstado}.");
        }

        var transicionValida =
            estadoActual == EstadoTrabajo.Pendiente &&
            nuevoEstado is EstadoTrabajo.EnProceso or EstadoTrabajo.Finalizado or EstadoTrabajo.Cancelado ||
            estadoActual == EstadoTrabajo.EnProceso &&
            nuevoEstado is EstadoTrabajo.Finalizado or EstadoTrabajo.Cancelado;

        if (!transicionValida)
        {
            throw new InvalidOperationException(
                $"La transicion de {estadoActual} a {nuevoEstado} no esta permitida.");
        }
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
