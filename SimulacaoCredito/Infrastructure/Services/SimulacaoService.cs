using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Domain.Entities;
using SimulacaoCredito.Infrastructure.Data;

namespace SimulacaoCredito.Infrastructure.Services;

public class SimulacaoService : ISimulacaoService
{
    private readonly AppDbContext _context;
    private readonly IProdutoService _produtoService;
    private readonly IAmortizacaoService _amortizacaoService;
    private readonly IEventHubService _eventHubService;

    public SimulacaoService(
        AppDbContext context,
        IProdutoService produtoService,
        IAmortizacaoService amortizacaoService,
        IEventHubService eventHubService)
    {
        _context = context;
        _produtoService = produtoService;
        _amortizacaoService = amortizacaoService;
        _eventHubService = eventHubService;
    }

    public async Task<SimulacaoResponseDto> CriarSimulacaoAsync(SimulacaoRequestDto request)
    {
        // Buscar produtos elegíveis
        var produtosElegiveis = await _produtoService.ObterProdutosElegiveisAsync(request.ValorDesejado, request.Prazo);
        
        if (!produtosElegiveis.Any())
        {
            throw new InvalidOperationException("Nenhum produto disponível para os parâmetros informados.");
        }

        // Selecionar o produto com menor taxa de juros
        var produtoSelecionado = produtosElegiveis.OrderBy(p => p.TaxaJuros).First();

        // Calcular amortizações
        var resultadosAmortizacao = _amortizacaoService.CalcularAmortizacoes(
            request.ValorDesejado, 
            produtoSelecionado.TaxaJuros, 
            request.Prazo);

        // Calcular valor total das parcelas (usando SAC como referência)
        var valorTotalParcelas = resultadosAmortizacao
            .First(r => r.Tipo == "SAC")
            .Parcelas.Sum(p => p.ValorPrestacao);

        // Criar simulação
        var simulacao = new Simulacao
        {
            DataCriacao = DateTimeOffset.Now,
            ValorDesejado = request.ValorDesejado,
            Prazo = request.Prazo,
            CodigoProduto = produtoSelecionado.CodigoProduto,
            DescricaoProduto = produtoSelecionado.NomeProduto,
            TaxaJuros = produtoSelecionado.TaxaJuros,
            ValorTotalParcelas = valorTotalParcelas
        };

        _context.Simulacoes.Add(simulacao);
        await _context.SaveChangesAsync();

        // Salvar parcelas
        foreach (var resultado in resultadosAmortizacao)
        {
            var tipoAmortizacao = resultado.Tipo == "SAC" ? TipoAmortizacao.SAC : TipoAmortizacao.PRICE;
            
            foreach (var parcelaDto in resultado.Parcelas)
            {
                var parcela = new Parcela
                {
                    SimulacaoId = simulacao.Id,
                    TipoAmortizacao = tipoAmortizacao,
                    Numero = parcelaDto.Numero,
                    ValorAmortizacao = parcelaDto.ValorAmortizacao,
                    ValorJuros = parcelaDto.ValorJuros,
                    ValorPrestacao = parcelaDto.ValorPrestacao
                };
                
                _context.Parcelas.Add(parcela);
            }
        }

        await _context.SaveChangesAsync();

        // Publicar evento no EventHub
        try
        {
            await _eventHubService.PublicarSimulacaoCriadaAsync(
                simulacao.Id,
                simulacao.ValorDesejado,
                simulacao.Prazo,
                simulacao.CodigoProduto);
        }
        catch (Exception ex)
        {
            // Log do erro mas não falha a operação
            // O evento será publicado em background ou retry
            Console.WriteLine($"Erro ao publicar evento: {ex.Message}");
        }

        // Retornar resposta
        return new SimulacaoResponseDto
        {
            IdSimulacao = simulacao.Id,
            CodigoProduto = simulacao.CodigoProduto,
            DescricaoProduto = simulacao.DescricaoProduto,
            TaxaJuros = simulacao.TaxaJuros,
            ResultadoSimulacao = resultadosAmortizacao
        };
    }

    public async Task<SimulacaoResponseDto?> ObterSimulacaoPorIdAsync(long id)
    {
        var simulacao = await _context.Simulacoes
            .Include(s => s.Parcelas)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (simulacao == null)
            return null;

        // Agrupar parcelas por tipo de amortização
        var resultadosAmortizacao = simulacao.Parcelas
            .GroupBy(p => p.TipoAmortizacao)
            .Select(g => new ResultadoAmortizacaoDto
            {
                Tipo = g.Key.ToString(),
                Parcelas = g.OrderBy(p => p.Numero).Select(p => new ParcelaDto
                {
                    Numero = p.Numero,
                    ValorAmortizacao = p.ValorAmortizacao,
                    ValorJuros = p.ValorJuros,
                    ValorPrestacao = p.ValorPrestacao
                }).ToList()
            }).ToList();

        return new SimulacaoResponseDto
        {
            IdSimulacao = simulacao.Id,
            CodigoProduto = simulacao.CodigoProduto,
            DescricaoProduto = simulacao.DescricaoProduto,
            TaxaJuros = simulacao.TaxaJuros,
            ResultadoSimulacao = resultadosAmortizacao
        };
    }

    public async Task<ListaSimulacoesResponseDto> ListarSimulacoesAsync(int pagina = 1, int tamanhoPagina = 10)
    {
        var skip = (pagina - 1) * tamanhoPagina;
        
        var totalRegistros = await _context.Simulacoes.CountAsync();
        
        var simulacoes = await _context.Simulacoes
            .OrderByDescending(s => s.DataCriacao)
            .Skip(skip)
            .Take(tamanhoPagina)
            .Select(s => new SimulacaoResumoDto
            {
                IdSimulacao = s.Id,
                ValorDesejado = s.ValorDesejado,
                Prazo = s.Prazo,
                ValorTotalParcelas = s.ValorTotalParcelas
            })
            .ToListAsync();

        return new ListaSimulacoesResponseDto
        {
            Pagina = pagina,
            QtdRegistros = totalRegistros,
            QtdRegistrosPagina = simulacoes.Count,
            Registros = simulacoes
        };
    }

    public async Task<VolumePorProdutoDiaResponseDto> ObterVolumePorProdutoDiaAsync(DateTime dataReferencia)
    {
        var dataInicio = dataReferencia.Date;
        var dataFim = dataInicio.AddDays(1);

        var simulacoesDia = await _context.Simulacoes
            .Where(s => s.DataCriacao >= dataInicio && s.DataCriacao < dataFim)
            .GroupBy(s => new { s.CodigoProduto, s.DescricaoProduto })
            .Select(g => new VolumeProdutoDto
            {
                CodigoProduto = g.Key.CodigoProduto,
                DescricaoProduto = g.Key.DescricaoProduto,
                TaxaMediaJuro = g.Average(s => s.TaxaJuros),
                ValorMedioPrestacao = g.Average(s => s.ValorTotalParcelas / s.Prazo),
                ValorTotalDesejado = g.Sum(s => s.ValorDesejado),
                ValorTotalCredito = g.Sum(s => s.ValorTotalParcelas)
            })
            .ToListAsync();

        return new VolumePorProdutoDiaResponseDto
        {
            DataReferencia = dataReferencia.ToString("yyyy-MM-dd"),
            Simulacoes = simulacoesDia
        };
    }
}
