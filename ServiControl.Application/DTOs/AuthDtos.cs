using ServiControl.Domain.Enums;

namespace ServiControl.Application.DTOs;

public record RegisterUserRequestDto(
    string Nombre,
    string Email,
    string Password,
    RolUsuario Rol);

public record LoginRequestDto(
    string Email,
    string Password);

public record UserResponseDto(
    int Id,
    string Nombre,
    string Email,
    RolUsuario Rol);

public record LoginResponseDto(
    string Token,
    int IdUsuario,
    string Nombre,
    string Email,
    RolUsuario Rol);
