using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Application.Interfaces;

namespace SimulacaoCredito.Controllers;

[ApiController]
[Route("api/v1")]
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
        try
        {
            var produtos = await _produtoService.ObterTodosProdutosAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar produtos");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("produtos/elegiveis")]
    public async Task<ActionResult> ObterProdutosElegiveis([FromQuery] decimal valor, [FromQuery] int prazo)
    {
        try
        {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos elegíveis para valor {Valor} e prazo {Prazo}", valor, prazo);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
