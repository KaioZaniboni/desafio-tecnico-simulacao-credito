using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SimulacaoCredito.Controllers;

[ApiController]
[Route("")]
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

    /// <summary>
    /// Cria uma nova simulação de crédito
    /// </summary>
    /// <param name="request">Dados da simulação</param>
    /// <returns>Resultado da simulação com cálculos SAC e PRICE</returns>
    [HttpPost("simulacoes")]
    [ProducesResponseType(typeof(SimulacaoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SimulacaoResponseDto>> CriarSimulacao([FromBody] SimulacaoRequestDto request)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de simulação - Valor: {Valor}, Prazo: {Prazo}", 
                request.ValorDesejado, request.Prazo);

            var resultado = await _simulacaoService.CriarSimulacaoAsync(request);
            
            _logger.LogInformation("Simulação criada com sucesso - ID: {Id}", resultado.IdSimulacao);
            
            return CreatedAtAction(nameof(ObterSimulacao), new { id = resultado.IdSimulacao }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro de validação na simulação: {Erro}", ex.Message);
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

    /// <summary>
    /// Obtém uma simulação por ID
    /// </summary>
    /// <param name="id">ID da simulação</param>
    /// <returns>Dados completos da simulação</returns>
    [HttpGet("simulacoes/{id:long}")]
    [ProducesResponseType(typeof(SimulacaoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Lista simulações com paginação
    /// </summary>
    /// <param name="pagina">Número da página (padrão: 1)</param>
    /// <param name="tamanhoPagina">Tamanho da página (padrão: 10, máximo: 100)</param>
    /// <returns>Lista paginada de simulações</returns>
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

    /// <summary>
    /// Obtém volume de simulações por produto em uma data específica
    /// </summary>
    /// <param name="dataReferencia">Data de referência (formato: yyyy-MM-dd)</param>
    /// <returns>Volume de simulações agrupado por produto</returns>
    [HttpGet("simulacoes/por-produto")]
    [ProducesResponseType(typeof(VolumePorProdutoDiaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
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
