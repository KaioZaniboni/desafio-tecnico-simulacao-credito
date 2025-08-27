using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;

namespace SimulacaoCredito.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class TelemetriaController : ControllerBase
{
    private readonly ITelemetriaService _telemetriaService;
    private readonly ILogger<TelemetriaController> _logger;

    public TelemetriaController(ITelemetriaService telemetriaService, ILogger<TelemetriaController> logger)
    {
        _telemetriaService = telemetriaService;
        _logger = logger;
    }

    [HttpGet("telemetria")]
    public async Task<ActionResult<TelemetriaResponseDto>> ObterTelemetriaDia(
        [FromQuery] DateTime dataReferencia)
    {
        try
        {
            var resultado = await _telemetriaService.ObterTelemetriaDiaAsync(dataReferencia);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter telemetria para {Data}", dataReferencia);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
