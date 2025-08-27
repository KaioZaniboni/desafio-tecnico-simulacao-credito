using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Application.Contracts;

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
        // Validação usando Flunt
        var contract = new TelemetriaContract(dataReferencia);
        if (!contract.IsValid)
        {
            var errors = contract.Notifications.Select(n => new
            {
                Property = n.Key,
                Message = n.Message
            }).ToList();

            return BadRequest(new ValidationProblemDetails
            {
                Title = "Data inválida",
                Detail = "A data de referência informada é inválida",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { "errors", errors } }
            });
        }

        var resultado = await _telemetriaService.ObterTelemetriaDiaAsync(dataReferencia);
        return Ok(resultado);
    }
}
