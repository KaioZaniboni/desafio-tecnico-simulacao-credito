using SimulacaoCredito.Domain.Entities;

namespace SimulacaoCredito.Application.Interfaces;

public interface IProdutoService
{
    Task<List<Produto>> ObterTodosProdutosAsync();
    Task<Produto?> ObterProdutoPorIdAsync(int codigoProduto);
    Task<List<Produto>> ObterProdutosElegiveisAsync(decimal valorDesejado, int prazo);
}
