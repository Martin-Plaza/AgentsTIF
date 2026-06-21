using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;

namespace ServiControl.Application.Services;

// Modulo: Usuarios
// Capa: Application
// Responsabilidad: Gestiona datos y roles de usuarios mediante contratos de persistencia.
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IReadOnlyList<UserResponseDto>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarioRepository.GetAllAsync(cancellationToken);
        return usuarios.Select(MapToResponse).ToList();
    }

    public async Task<UserResponseDto> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var usuario = await ObtenerEntidadAsync(id, cancellationToken);
        return MapToResponse(usuario);
    }

    public async Task<UserResponseDto> ActualizarAsync(
        int id,
        UpdateUsuarioRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var usuario = await ObtenerEntidadAsync(id, cancellationToken);
        var email = request.Email.Trim();
        var usuarioConEmail = await _usuarioRepository.GetByEmailAsync(email, cancellationToken);

        if (usuarioConEmail is not null && usuarioConEmail.Id != id)
        {
            throw new InvalidOperationException("El email ya esta registrado.");
        }

        usuario.ActualizarDatos(request.Nombre, email);
        await _usuarioRepository.UpdateAsync(usuario, cancellationToken);

        return MapToResponse(usuario);
    }

    public async Task<UserResponseDto> CambiarRolAsync(
        int id,
        UpdateUsuarioRolRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var usuario = await ObtenerEntidadAsync(id, cancellationToken);
        usuario.CambiarRol(request.Rol);
        await _usuarioRepository.UpdateAsync(usuario, cancellationToken);

        return MapToResponse(usuario);
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        var usuario = await ObtenerEntidadAsync(id, cancellationToken);

        if (await _usuarioRepository.HasRelatedRecordsAsync(id, cancellationToken))
        {
            throw new InvalidOperationException(
                "No se puede eliminar el usuario porque tiene trabajos o metricas relacionados.");
        }

        await _usuarioRepository.DeleteAsync(usuario, cancellationToken);
    }

    private async Task<Usuario> ObtenerEntidadAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _usuarioRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ArgumentException("El usuario indicado no existe.", nameof(id));
    }

    private static UserResponseDto MapToResponse(Usuario usuario)
    {
        return new UserResponseDto(usuario.Id, usuario.Nombre, usuario.Email, usuario.Rol);
    }
}
