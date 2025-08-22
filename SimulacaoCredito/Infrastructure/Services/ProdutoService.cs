using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Domain.Entities;
using SimulacaoCredito.Infrastructure.Data;

namespace SimulacaoCredito.Infrastructure.Services;

public class ProdutoService : IProdutoService
{
    private readonly ProdutoDbContext _context;

    public ProdutoService(ProdutoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Produto>> ObterTodosProdutosAsync()
    {
        return await _context.Produtos.ToListAsync();
    }

    public async Task<Produto?> ObterProdutoPorIdAsync(int codigoProduto)
    {
        return await _context.Produtos.FindAsync(codigoProduto);
    }

    public async Task<List<Produto>> ObterProdutosElegiveisAsync(decimal valorDesejado, int prazo)
    {
        return await _context.Produtos
            .Where(p => valorDesejado >= p.ValorMinimo &&
                       (p.ValorMaximo == null || valorDesejado <= p.ValorMaximo) &&
                       prazo >= p.MinimoMeses &&
                       (p.MaximoMeses == null || prazo <= p.MaximoMeses))
            .ToListAsync();
    }
}
