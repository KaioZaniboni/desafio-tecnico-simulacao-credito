using SimulacaoCredito.Infrastructure.Services;
using Xunit;

namespace SimulacaoCredito.Tests;

public class AmortizacaoServiceTests
{
    private readonly AmortizacaoService _amortizacaoService;

    public AmortizacaoServiceTests()
    {
        _amortizacaoService = new AmortizacaoService();
    }

    [Fact]
    public void CalcularSAC_DeveRetornarParcelasComAmortizacaoConstante()
    {
        // Arrange
        decimal valorPrincipal = 100000m;
        decimal taxaJurosAnual = 12m; // 12% ao ano
        int numeroParcelas = 12;

        // Act
        var parcelas = _amortizacaoService.CalcularSAC(valorPrincipal, taxaJurosAnual, numeroParcelas);

        // Assert
        Assert.Equal(numeroParcelas, parcelas.Count);

        // Todas as parcelas devem ter a mesma amortização
        var amortizacaoEsperada = Math.Round(valorPrincipal / numeroParcelas, 2);
        Assert.All(parcelas, p => Assert.Equal(amortizacaoEsperada, p.ValorAmortizacao));

        // A primeira parcela deve ter o maior valor de juros
        Assert.True(parcelas[0].ValorJuros > parcelas[^1].ValorJuros);

        // A primeira parcela deve ter a maior prestação
        Assert.True(parcelas[0].ValorPrestacao > parcelas[^1].ValorPrestacao);
    }

    [Fact]
    public void CalcularPRICE_DeveRetornarParcelasComPrestacaoConstante()
    {
        // Arrange
        decimal valorPrincipal = 100000m;
        decimal taxaJurosAnual = 12m; // 12% ao ano
        int numeroParcelas = 12;

        // Act
        var parcelas = _amortizacaoService.CalcularPRICE(valorPrincipal, taxaJurosAnual, numeroParcelas);

        // Assert
        Assert.Equal(numeroParcelas, parcelas.Count);

        // Todas as parcelas devem ter a mesma prestação (com tolerância para arredondamento)
        var prestacaoEsperada = parcelas[0].ValorPrestacao;
        Assert.All(parcelas, p => Assert.Equal(prestacaoEsperada, p.ValorPrestacao));

        // A primeira parcela deve ter menor amortização que a última
        Assert.True(parcelas[0].ValorAmortizacao < parcelas[^1].ValorAmortizacao);

        // A primeira parcela deve ter maior valor de juros que a última
        Assert.True(parcelas[0].ValorJuros > parcelas[^1].ValorJuros);
    }

    [Fact]
    public void CalcularAmortizacoes_DeveRetornarAmbosOsSistemas()
    {
        // Arrange
        decimal valorPrincipal = 50000m;
        decimal taxaJurosAnual = 10m;
        int numeroParcelas = 6;

        // Act
        var resultados = _amortizacaoService.CalcularAmortizacoes(valorPrincipal, taxaJurosAnual, numeroParcelas);

        // Assert
        Assert.Equal(2, resultados.Count);
        Assert.Contains(resultados, r => r.Tipo == "SAC");
        Assert.Contains(resultados, r => r.Tipo == "PRICE");

        var resultadoSAC = resultados.First(r => r.Tipo == "SAC");
        var resultadoPRICE = resultados.First(r => r.Tipo == "PRICE");

        Assert.Equal(numeroParcelas, resultadoSAC.Parcelas.Count);
        Assert.Equal(numeroParcelas, resultadoPRICE.Parcelas.Count);
    }

    [Theory]
    [InlineData(1000, 12, 1)]
    [InlineData(50000, 6, 12)]
    [InlineData(100000, 24, 24)]
    public void CalcularSAC_ComDiferentesParametros_DeveCalcularCorretamente(decimal valor, decimal taxa, int parcelas)
    {
        // Act
        var resultado = _amortizacaoService.CalcularSAC(valor, taxa, parcelas);

        // Assert
        Assert.Equal(parcelas, resultado.Count);
        Assert.All(resultado, p => Assert.True(p.ValorPrestacao > 0));
        Assert.All(resultado, p => Assert.True(p.ValorAmortizacao > 0));
        Assert.All(resultado, p => Assert.True(p.ValorJuros >= 0));

        // Soma das amortizações deve ser igual ao valor principal (com tolerância)
        var somaAmortizacoes = resultado.Sum(p => p.ValorAmortizacao);
        Assert.True(Math.Abs(somaAmortizacoes - valor) < 1m);
    }
}