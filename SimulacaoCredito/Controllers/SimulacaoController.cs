using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Application.Contracts;
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
        // Validação usando Flunt
        if (!request.IsValid())
        {
            var errors = request.Notifications.Select(n => new
            {
                Property = n.Key,
                Message = n.Message
            }).ToList();

            return BadRequest(new ValidationProblemDetails
            {
                Title = "Dados inválidos",
                Detail = "Um ou mais campos contêm valores inválidos",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { "errors", errors } }
            });
        }

        try
        {
            _logger.LogInformation("Iniciando criação de simulação. ValorDesejado: {ValorDesejado}, Prazo: {Prazo}",
                request.ValorDesejado, request.Prazo);

            var resultado = await _simulacaoService.CriarSimulacaoAsync(request);

            _logger.LogInformation("Simulação criada com sucesso. ID: {IdSimulacao}, Produto: {CodigoProduto}, ValorTotal: {ValorTotal}",
                resultado.IdSimulacao, resultado.CodigoProduto,
                resultado.ResultadoSimulacao.FirstOrDefault()?.Parcelas.Sum(p => p.ValorPrestacao));

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
    }

    [HttpGet("simulacoes/{id:long}")]
    public async Task<ActionResult<SimulacaoResponseDto>> ObterSimulacao(long id)
    {
        _logger.LogInformation("Buscando simulação por ID: {IdSimulacao}", id);

        var simulacao = await _simulacaoService.ObterSimulacaoPorIdAsync(id);

        if (simulacao == null)
        {
            _logger.LogWarning("Simulação não encontrada. ID: {IdSimulacao}", id);
            return NotFound(new ProblemDetails
            {
                Title = "Simulação não encontrada",
                Detail = $"Não foi encontrada simulação com ID {id}",
                Status = StatusCodes.Status404NotFound
            });
        }

        _logger.LogInformation("Simulação encontrada. ID: {IdSimulacao}, Produto: {CodigoProduto}",
            simulacao.IdSimulacao, simulacao.CodigoProduto);
        return Ok(simulacao);
    }

    [HttpGet("simulacoes")]
    [ProducesResponseType(typeof(ListaSimulacoesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListaSimulacoesResponseDto>> ListarSimulacoes(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        // Validação usando Flunt
        var contract = new PaginacaoContract(pagina, tamanhoPagina);
        if (!contract.IsValid)
        {
            var errors = contract.Notifications.Select(n => new
            {
                Property = n.Key,
                Message = n.Message
            }).ToList();

            return BadRequest(new ValidationProblemDetails
            {
                Title = "Parâmetros de paginação inválidos",
                Detail = "Os parâmetros de paginação informados são inválidos",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { "errors", errors } }
            });
        }

        var resultado = await _simulacaoService.ListarSimulacoesAsync(pagina, tamanhoPagina);
        return Ok(resultado);
    }

    [HttpGet("simulacoes/por-produto")]
    public async Task<ActionResult<VolumePorProdutoDiaResponseDto>> ObterVolumePorProdutoDia(
        [FromQuery] DateTime dataReferencia)
    {
        var resultado = await _simulacaoService.ObterVolumePorProdutoDiaAsync(dataReferencia);
        return Ok(resultado);
    }
}
