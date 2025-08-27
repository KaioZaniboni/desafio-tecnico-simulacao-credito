using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Application.Contracts;
using Flunt.Notifications;

namespace SimulacaoCredito.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
[Produces("application/json")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ProdutoController> _logger;

    public ProdutoController(IProdutoService produtoService, ILogger<ProdutoController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    [HttpGet("produtos")]
    public async Task<ActionResult> ListarProdutos()
    {
        var produtos = await _produtoService.ObterTodosProdutosAsync();
        return Ok(produtos);
    }

    [HttpGet("produtos/elegiveis")]
    public async Task<ActionResult> ObterProdutosElegiveis([FromQuery] decimal valor, [FromQuery] int prazo)
    {
        // Validação usando Flunt
        var contract = new ProdutoElegivelContract(valor, prazo);
        if (!contract.IsValid)
        {
            var errors = contract.Notifications.Select(n => new
            {
                Property = n.Key,
                Message = n.Message
            }).ToList();

            return BadRequest(new ValidationProblemDetails
            {
                Title = "Parâmetros inválidos",
                Detail = "Um ou mais parâmetros contêm valores inválidos",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { "errors", errors } }
            });
        }

        var produtosElegiveis = await _produtoService.ObterProdutosElegiveisAsync(valor, prazo);

        var resultado = new
        {
            Valor = valor,
            Prazo = prazo,
            ProdutosElegiveis = produtosElegiveis,
            Quantidade = produtosElegiveis.Count
        };

        return Ok(resultado);
    }
}
