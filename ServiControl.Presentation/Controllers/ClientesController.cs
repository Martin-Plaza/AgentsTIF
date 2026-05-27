using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Presentation.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResponse>> Crear(
        CreateClienteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await _clienteService.CrearAsync(request, cancellationToken);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.Id }, cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteResponse>>> ObtenerTodos(
        CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.ObtenerTodosAsync(cancellationToken);
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> ObtenerPorId(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id, cancellationToken);
            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> Actualizar(
        int id,
        UpdateClienteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await _clienteService.ActualizarAsync(id, request, cancellationToken);
            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
