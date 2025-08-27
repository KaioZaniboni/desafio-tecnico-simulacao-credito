using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Domain.Entities;

namespace SimulacaoCredito.Infrastructure.Services;

public class AmortizacaoService : IAmortizacaoService
{
    public List<ParcelaDto> CalcularSAC(decimal valorPrincipal, decimal taxaJurosAnual, int numeroParcelas)
    {
        var parcelas = new List<ParcelaDto>();
        // Taxa vem em formato decimal anual (ex: 0.0179 para 1.79% ao ano)
        // Convertendo para taxa mensal: taxa_anual / 12
        var taxaJurosMensal = taxaJurosAnual / 12;
        var valorAmortizacao = valorPrincipal / numeroParcelas;
        var saldoDevedor = valorPrincipal;

        for (int i = 1; i <= numeroParcelas; i++)
        {
            var valorJuros = saldoDevedor * taxaJurosMensal;
            var valorPrestacao = valorAmortizacao + valorJuros;
            
            parcelas.Add(new ParcelaDto
            {
                Numero = i,
                ValorAmortizacao = Math.Round(valorAmortizacao, 2),
                ValorJuros = Math.Round(valorJuros, 2),
                ValorPrestacao = Math.Round(valorPrestacao, 2)
            });

            saldoDevedor -= valorAmortizacao;
        }

        return parcelas;
    }

    public List<ParcelaDto> CalcularPRICE(decimal valorPrincipal, decimal taxaJurosAnual, int numeroParcelas)
    {
        var parcelas = new List<ParcelaDto>();
        // Taxa vem em formato decimal anual (ex: 0.0179 para 1.79% ao ano)
        // Convertendo para taxa mensal: taxa_anual / 12
        var taxaJurosMensal = taxaJurosAnual / 12;

        var prestacaoFixa = valorPrincipal * (taxaJurosMensal * (decimal)Math.Pow((double)(1 + taxaJurosMensal), numeroParcelas)) /
                           ((decimal)Math.Pow((double)(1 + taxaJurosMensal), numeroParcelas) - 1);
        
        var saldoDevedor = valorPrincipal;

        for (int i = 1; i <= numeroParcelas; i++)
        {
            var valorJuros = saldoDevedor * taxaJurosMensal;
            var valorAmortizacao = prestacaoFixa - valorJuros;
            
            parcelas.Add(new ParcelaDto
            {
                Numero = i,
                ValorAmortizacao = Math.Round(valorAmortizacao, 2),
                ValorJuros = Math.Round(valorJuros, 2),
                ValorPrestacao = Math.Round(prestacaoFixa, 2)
            });

            saldoDevedor -= valorAmortizacao;
        }

        return parcelas;
    }

    public List<ResultadoAmortizacaoDto> CalcularAmortizacoes(decimal valorPrincipal, decimal taxaJurosAnual, int numeroParcelas)
    {
        var resultados = new List<ResultadoAmortizacaoDto>();

        var parcelasSAC = CalcularSAC(valorPrincipal, taxaJurosAnual, numeroParcelas);
        resultados.Add(new ResultadoAmortizacaoDto
        {
            Tipo = "SAC",
            Parcelas = parcelasSAC
        });

        var parcelasPRICE = CalcularPRICE(valorPrincipal, taxaJurosAnual, numeroParcelas);
        resultados.Add(new ResultadoAmortizacaoDto
        {
            Tipo = "PRICE",
            Parcelas = parcelasPRICE
        });

        return resultados;
    }
}
