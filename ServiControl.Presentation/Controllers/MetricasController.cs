using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Presentation.Controllers;

[ApiController]
[Route("api/metricas")]
public class MetricasController : ControllerBase
{
    private readonly IMetricaService _metricaService;

    public MetricasController(IMetricaService metricaService)
    {
        _metricaService = metricaService;
    }

    [HttpGet]
    public async Task<ActionResult<MetricaResponse>> GenerarPorRango(
        [FromQuery] int usuarioId,
        [FromQuery] DateTime periodoInicio,
        [FromQuery] DateTime periodoFin,
        CancellationToken cancellationToken)
    {
        try
        {
            var metrica = await _metricaService.GenerarPorRangoAsync(
                new GenerateMetricaRequest(usuarioId, periodoInicio, periodoFin),
                cancellationToken);

            return Ok(metrica);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
