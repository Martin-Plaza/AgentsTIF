using ServiControl.Domain.Enums;

namespace ServiControl.Domain.Entities;

public class Usuario
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public RolUsuario Rol { get; private set; }

    public Usuario(string nombre, string email, string passwordHash, RolUsuario rol)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email del usuario es obligatorio.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("La password del usuario es obligatoria.", nameof(passwordHash));
        }

        Nombre = nombre;
        Email = email;
        PasswordHash = passwordHash;
        Rol = rol;
    }

    public void ActualizarDatos(string nombre, string email)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email del usuario es obligatorio.", nameof(email));
        }

        Nombre = nombre;
        Email = email;
    }

    public void CambiarRol(RolUsuario rol)
    {
        Rol = rol;
    }
}
