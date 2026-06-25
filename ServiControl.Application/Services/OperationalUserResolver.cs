using ServiControl.Application.Authorization;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Enums;

namespace ServiControl.Application.Services;

// Modulo: Autorizacion operativa
// Capa: Application
// Responsabilidad: Decide que UsuarioId se usa para trabajos, costos y metricas.
public class OperationalUserResolver : IOperationalUserResolver
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUsuarioRepository _usuarioRepository;

    public OperationalUserResolver(
        ICurrentUserContext currentUserContext,
        IUsuarioRepository usuarioRepository)
    {
        _currentUserContext = currentUserContext;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<int> ObtenerUsuarioOperativoIdAsync(
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(
            _currentUserContext.UsuarioId,
            cancellationToken)
            ?? throw new UnauthorizedAccessException("El usuario autenticado no existe.");

        if (usuario.Rol == RolUsuario.Assistant)
        {
            if (!usuario.IdUsuarioResponsable.HasValue)
            {
                throw new InvalidOperationException(
                    "El asistente no tiene un tecnico responsable asignado.");
            }

            var responsable = await _usuarioRepository.GetByIdAsync(
                usuario.IdUsuarioResponsable.Value,
                cancellationToken)
                ?? throw new InvalidOperationException(
                    "El tecnico responsable asignado no existe.");

            if (responsable.Rol != RolUsuario.Technician)
            {
                throw new InvalidOperationException(
                    "El usuario responsable asignado no tiene rol Tecnico.");
            }

            return responsable.Id;
        }

        return usuario.Id;
    }
}
