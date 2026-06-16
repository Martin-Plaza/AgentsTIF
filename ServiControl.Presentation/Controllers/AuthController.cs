using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Presentation.Controllers;

// Modulo: Autenticacion
// Capa: Presentation
// Responsabilidad: Expone registro y login; la logica vive en Application.
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(
        RegisterUserRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _authService.RegisterAsync(request, cancellationToken);
            //string.Empty reemplaza a la url del recurso creado
            return Created(string.Empty, user);
        }
        catch (ArgumentException ex)
        {
            //bad request es por un un mail invalido por ejemplo
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            //es cuando esta bien la request pero hay conflicto en la logica del sistema (por ejemplo un mail existente)
            return Conflict(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            //sin autorizacion el usuario
            return Unauthorized(ex.Message);
        }
    }
}
