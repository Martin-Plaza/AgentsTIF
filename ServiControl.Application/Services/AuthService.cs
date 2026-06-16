using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;

namespace ServiControl.Application.Services;

// Modulo: Autenticacion
// Capa: Application
// Responsabilidad: Orquesta registro y login sin acoplarse a infraestructura concreta.
// Nota: La generacion de JWT y el hashing se consumen mediante abstracciones.
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<UserResponseDto> RegisterAsync(
        RegisterUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("La password es obligatoria.", nameof(request.Password));
        }

        var email = request.Email.Trim();

        if (await _usuarioRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new InvalidOperationException("El email ya esta registrado.");
        }

        //hashea password
        var passwordHash = _passwordHasher.Hash(request.Password);
        //crea instancia de usuario (capa dominio)
        var usuario = new Usuario(request.Nombre, email, passwordHash, request.Rol);
        //ingresa el usuario en el repositorio
        var created = await _usuarioRepository.AddAsync(usuario, cancellationToken);

        //le pasamos el DTO para mostrarlo
        return MapToResponse(created);
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Email o password invalidos.");
        }

        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);

        if (usuario is null || !_passwordHasher.Verify(request.Password, usuario.PasswordHash))
        {
            throw new InvalidOperationException("Email o password invalidos.");
        }

        var token = _jwtTokenGenerator.GenerateToken(usuario);

        return new LoginResponseDto(
            token,
            usuario.Id,
            usuario.Nombre,
            usuario.Email,
            usuario.Rol);
    }

    private static UserResponseDto MapToResponse(Usuario usuario)
    {
        return new UserResponseDto(
            usuario.Id,
            usuario.Nombre,
            usuario.Email,
            usuario.Rol);
    }
}
