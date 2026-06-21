using ServiControl.Domain.Enums;

namespace ServiControl.Application.DTOs;

public record UpdateUsuarioRequestDto(
    string Nombre,
    string Email);

public record UpdateUsuarioRolRequestDto(
    RolUsuario Rol);
