using ServiControl.Domain.Enums;

namespace ServiControl.Application.Authorization;

// Centraliza los nombres que se guardan en el claim de rol del JWT.
public static class Roles
{
    public const string Admin = nameof(RolUsuario.Admin);
    public const string Tecnico = nameof(RolUsuario.Technician);
    public const string Asistente = nameof(RolUsuario.Assistant);

    public const string AdminTecnico = Admin + "," + Tecnico;
    public const string Todos = Admin + "," + Tecnico + "," + Asistente;
}
