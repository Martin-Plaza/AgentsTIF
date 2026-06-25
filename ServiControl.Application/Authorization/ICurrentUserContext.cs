namespace ServiControl.Application.Authorization;

// Expone identidad y privilegio global sin acoplar Application a HttpContext.
public interface ICurrentUserContext
{
    int UsuarioId { get; }
    bool IsAdmin { get; }
}
