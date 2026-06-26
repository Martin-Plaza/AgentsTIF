using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;

namespace ServiControl.Application.Services;

// Modulo: Clientes
// Capa: Application
// Responsabilidad: Coordina casos de uso de clientes y devuelve DTOs a la API.
public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteResponse> CrearAsync(
        CreateClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        //crea un cliente de dominio (en el return devolvemos el DTO creado al final de este modulo MapToResponse)
        var cliente = new Cliente(
            request.Nombre,
            request.Telefono,
            request.Email,
            request.Observaciones);

        var created = await _clienteRepository.AddAsync(cliente, cancellationToken);

        //funcion para devolver DTO creada abajo de este archivo
        return MapToResponse(created);
    }

    public async Task<IReadOnlyList<ClienteResponse>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.GetAllAsync(cancellationToken);

        return clientes.Select(MapToResponse).ToList();
    }

    public async Task<ClienteResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        //si devuelve algo el repositorio, es decir, se obtuvo algun ID se guarda, sino arroja la excepcion (por eso el "??")
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ArgumentException("El cliente indicado no existe.", nameof(id));

        return MapToResponse(cliente);
    }

    public async Task<ClienteResponse> ActualizarAsync(
        int id,
        UpdateClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ArgumentException("El cliente indicado no existe.", nameof(id));

        cliente.ActualizarDatos(
            request.Nombre,
            request.Telefono,
            request.Email,
            request.Observaciones);

        await _clienteRepository.UpdateAsync(cliente, cancellationToken);

        return MapToResponse(cliente);
    }

    //creador del DTO para devolver respuesta al endPoint
    private static ClienteResponse MapToResponse(Cliente cliente)
    {
        return new ClienteResponse(
            cliente.Id,
            cliente.Nombre,
            cliente.Telefono,
            cliente.Email,
            cliente.Observaciones);
    }
}
