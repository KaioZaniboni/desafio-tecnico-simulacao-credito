using Microsoft.EntityFrameworkCore;
using Moq;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Domain.Entities;
using SimulacaoCredito.Infrastructure.Data;
using SimulacaoCredito.Infrastructure.Services;
using Xunit;

namespace SimulacaoCredito.Tests;

public class SimulacaoServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IProdutoService> _mockProdutoService;
    private readonly Mock<IEventHubService> _mockEventHubService;
    private readonly IAmortizacaoService _amortizacaoService;
    private readonly SimulacaoService _simulacaoService;

    public SimulacaoServiceTests()
    {
        // Configurar banco em memória
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        
        // Configurar mocks
        _mockProdutoService = new Mock<IProdutoService>();
        _mockEventHubService = new Mock<IEventHubService>();
        _amortizacaoService = new AmortizacaoService();
        
        _simulacaoService = new SimulacaoService(
            _context, 
            _mockProdutoService.Object, 
            _amortizacaoService,
            _mockEventHubService.Object);
    }

    [Fact]
    public async Task CriarSimulacaoAsync_ComProdutoElegivel_DeveCriarSimulacaoComSucesso()
    {
        // Arrange
        var request = new SimulacaoRequestDto
        {
            ValorDesejado = 50000m,
            Prazo = 12
        };

        var produtoElegivel = new Produto
        {
            CodigoProduto = 1,
            NomeProduto = "Crédito Pessoal",
            TaxaJuros = 1.5m,
            MinimoMeses = 6,
            MaximoMeses = 24,
            ValorMinimo = 1000m,
            ValorMaximo = 100000m
        };

        _mockProdutoService
            .Setup(x => x.ObterProdutosElegiveisAsync(request.ValorDesejado, request.Prazo))
            .ReturnsAsync(new List<Produto> { produtoElegivel });

        // Act
        var resultado = await _simulacaoService.CriarSimulacaoAsync(request);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.IdSimulacao > 0);
        Assert.Equal(produtoElegivel.CodigoProduto, resultado.CodigoProduto);
        Assert.Equal(produtoElegivel.NomeProduto, resultado.DescricaoProduto);
        Assert.Equal(produtoElegivel.TaxaJuros, resultado.TaxaJuros);
        Assert.Equal(2, resultado.ResultadoSimulacao.Count); // SAC e PRICE
        
        // Verificar se foi salvo no banco
        var simulacaoSalva = await _context.Simulacoes.FirstOrDefaultAsync();
        Assert.NotNull(simulacaoSalva);
        Assert.Equal(request.ValorDesejado, simulacaoSalva.ValorDesejado);
        Assert.Equal(request.Prazo, simulacaoSalva.Prazo);
        
        // Verificar se as parcelas foram salvas
        var parcelasSalvas = await _context.Parcelas.Where(p => p.SimulacaoId == simulacaoSalva.Id).ToListAsync();
        Assert.Equal(24, parcelasSalvas.Count); // 12 parcelas SAC + 12 parcelas PRICE
        
        // Verificar se o evento foi publicado
        _mockEventHubService.Verify(x => x.PublicarSimulacaoCriadaAsync(
            It.IsAny<long>(), 
            request.ValorDesejado, 
            request.Prazo, 
            produtoElegivel.CodigoProduto), Times.Once);
    }

    [Fact]
    public async Task CriarSimulacaoAsync_SemProdutoElegivel_DeveLancarExcecao()
    {
        // Arrange
        var request = new SimulacaoRequestDto
        {
            ValorDesejado = 1000000m, // Valor muito alto
            Prazo = 500 // Prazo muito longo
        };

        _mockProdutoService
            .Setup(x => x.ObterProdutosElegiveisAsync(request.ValorDesejado, request.Prazo))
            .ReturnsAsync(new List<Produto>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _simulacaoService.CriarSimulacaoAsync(request));
        
        Assert.Equal("Nenhum produto disponível para os parâmetros informados.", exception.Message);
    }

    [Fact]
    public async Task ObterSimulacaoPorIdAsync_ComIdExistente_DeveRetornarSimulacao()
    {
        // Arrange
        var simulacao = new Simulacao
        {
            ValorDesejado = 25000m,
            Prazo = 6,
            CodigoProduto = 2,
            DescricaoProduto = "Crédito Consignado",
            TaxaJuros = 1.2m,
            ValorTotalParcelas = 26500m,
            DataCriacao = DateTimeOffset.Now
        };

        _context.Simulacoes.Add(simulacao);
        await _context.SaveChangesAsync();

        // Adicionar algumas parcelas
        var parcelas = new List<Parcela>
        {
            new() { SimulacaoId = simulacao.Id, TipoAmortizacao = TipoAmortizacao.SAC, Numero = 1, ValorAmortizacao = 4000m, ValorJuros = 300m, ValorPrestacao = 4300m },
            new() { SimulacaoId = simulacao.Id, TipoAmortizacao = TipoAmortizacao.PRICE, Numero = 1, ValorAmortizacao = 3800m, ValorJuros = 300m, ValorPrestacao = 4100m }
        };

        _context.Parcelas.AddRange(parcelas);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _simulacaoService.ObterSimulacaoPorIdAsync(simulacao.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(simulacao.Id, resultado.IdSimulacao);
        Assert.Equal(simulacao.CodigoProduto, resultado.CodigoProduto);
        Assert.Equal(2, resultado.ResultadoSimulacao.Count);
    }

    [Fact]
    public async Task ObterSimulacaoPorIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Act
        var resultado = await _simulacaoService.ObterSimulacaoPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ListarSimulacoesAsync_ComPaginacao_DeveRetornarListaPaginada()
    {
        // Arrange
        var simulacoes = new List<Simulacao>();
        for (int i = 1; i <= 15; i++)
        {
            simulacoes.Add(new Simulacao
            {
                ValorDesejado = 10000m * i,
                Prazo = 12,
                CodigoProduto = 1,
                DescricaoProduto = $"Produto {i}",
                TaxaJuros = 1.5m,
                ValorTotalParcelas = 11000m * i,
                DataCriacao = DateTimeOffset.Now.AddDays(-i)
            });
        }

        _context.Simulacoes.AddRange(simulacoes);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _simulacaoService.ListarSimulacoesAsync(pagina: 2, tamanhoPagina: 5);

        // Assert
        Assert.Equal(2, resultado.Pagina);
        Assert.Equal(15, resultado.QtdRegistros);
        Assert.Equal(5, resultado.QtdRegistrosPagina);
        Assert.Equal(5, resultado.Registros.Count);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
