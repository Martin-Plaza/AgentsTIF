namespace ServiControl.Application.DTOs;

public record CreateClienteRequest(
    string Nombre,
    string Telefono,
    string? Email,
    string? Observaciones);

public record UpdateClienteRequest(
    string Nombre,
    string Telefono,
    string? Email,
    string? Observaciones);

public record ClienteResponse(
    int Id,
    string Nombre,
    string Telefono,
    string? Email,
    string? Observaciones);
