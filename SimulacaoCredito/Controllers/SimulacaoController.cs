using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SimulacaoCredito.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class SimulacaoController : ControllerBase
{
    private readonly ISimulacaoService _simulacaoService;
    private readonly ILogger<SimulacaoController> _logger;

    public SimulacaoController(ISimulacaoService simulacaoService, ILogger<SimulacaoController> logger)
    {
        _simulacaoService = simulacaoService;
        _logger = logger;
    }

    [HttpPost("simulacoes")]
    public async Task<ActionResult<SimulacaoResponseDto>> CriarSimulacao([FromBody] SimulacaoRequestDto request)
    {
        try
        {
            var resultado = await _simulacaoService.CriarSimulacaoAsync(request);
            return CreatedAtAction(nameof(ObterSimulacao), new { id = resultado.IdSimulacao }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Parâmetros inválidos",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao criar simulação");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro inesperado ao processar a simulação",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpGet("simulacoes/{id:long}")]
    public async Task<ActionResult<SimulacaoResponseDto>> ObterSimulacao(long id)
    {
        try
        {
            var simulacao = await _simulacaoService.ObterSimulacaoPorIdAsync(id);
            
            if (simulacao == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Simulação não encontrada",
                    Detail = $"Não foi encontrada simulação com ID {id}",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(simulacao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar simulação {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("simulacoes")]
    [ProducesResponseType(typeof(ListaSimulacoesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListaSimulacoesResponseDto>> ListarSimulacoes(
        [FromQuery, Range(1, int.MaxValue)] int pagina = 1,
        [FromQuery, Range(1, 100)] int tamanhoPagina = 10)
    {
        try
        {
            var resultado = await _simulacaoService.ListarSimulacoesAsync(pagina, tamanhoPagina);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar simulações");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("simulacoes/por-produto")]
    public async Task<ActionResult<VolumePorProdutoDiaResponseDto>> ObterVolumePorProdutoDia(
        [FromQuery] DateTime dataReferencia)
    {
        try
        {
            var resultado = await _simulacaoService.ObterVolumePorProdutoDiaAsync(dataReferencia);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter volume por produto/dia para {Data}", dataReferencia);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
