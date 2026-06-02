using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Presentation.Controllers;

// Modulo: Costos
// Capa: Presentation
// Responsabilidad: Expone operaciones de costos y delega validaciones a Application/Domain.
[ApiController]
[Authorize]
[Route("api/costos")]
public class CostosController : ControllerBase
{
    private readonly ICostoService _costoService;

    public CostosController(ICostoService costoService)
    {
        _costoService = costoService;
    }

    [HttpPost]
    public async Task<ActionResult<CostoResponse>> RegistrarCostoEstimado(
        RegisterCostoEstimadoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var costo = await _costoService.RegistrarCostoEstimadoAsync(request, cancellationToken);
            return Ok(costo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}/estimado")]
    public async Task<ActionResult<CostoResponse>> RegistrarCostoEstimado(
        int id,
        [FromBody] decimal costoEstimado,
        CancellationToken cancellationToken)
    {
        try
        {
            var costo = await _costoService.RegistrarCostoEstimadoAsync(
                new RegisterCostoEstimadoRequest(id, costoEstimado),
                cancellationToken);

            return Ok(costo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}/final")]
    public async Task<ActionResult<CostoResponse>> RegistrarCostoFinal(
        int id,
        [FromBody] decimal costoFinal,
        CancellationToken cancellationToken)
    {
        try
        {
            var costo = await _costoService.RegistrarCostoFinalAsync(
                new RegisterCostoFinalRequest(id, costoFinal),
                cancellationToken);

            return Ok(costo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
