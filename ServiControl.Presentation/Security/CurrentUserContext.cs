using System.Security.Claims;
using ServiControl.Application.Authorization;

namespace ServiControl.Presentation.Security;

// Adapta los claims del JWT al contrato de usuario actual definido en Application.
public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UsuarioId
    {
        get
        {
            var value = User.FindFirstValue(AuthClaimTypes.UsuarioId);

            if (!int.TryParse(value, out var usuarioId) || usuarioId <= 0)
            {
                throw new UnauthorizedAccessException("El token no contiene un UsuarioId valido.");
            }

            return usuarioId;
        }
    }

    public bool IsAdmin => User.IsInRole(Roles.Admin);

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("No existe un usuario autenticado.");
}
