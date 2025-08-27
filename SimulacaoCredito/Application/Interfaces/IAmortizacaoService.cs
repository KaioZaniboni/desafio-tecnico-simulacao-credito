using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Domain.Entities;

namespace SimulacaoCredito.Application.Interfaces;

public interface IAmortizacaoService
{
    /// <summary>
    /// Calcula as parcelas usando o Sistema de Amortização Constante (SAC)
    /// </summary>
    /// <param name="valorPrincipal">Valor principal do empréstimo</param>
    /// <param name="taxaJurosAnual">Taxa de juros anual em formato decimal (ex: 0.0179 para 1.79% a.a.)</param>
    /// <param name="numeroParcelas">Número de parcelas mensais</param>
    List<ParcelaDto> CalcularSAC(decimal valorPrincipal, decimal taxaJurosAnual, int numeroParcelas);

    /// <summary>
    /// Calcula as parcelas usando o Sistema Francês de Amortização (PRICE)
    /// </summary>
    /// <param name="valorPrincipal">Valor principal do empréstimo</param>
    /// <param name="taxaJurosAnual">Taxa de juros anual em formato decimal (ex: 0.0179 para 1.79% a.a.)</param>
    /// <param name="numeroParcelas">Número de parcelas mensais</param>
    List<ParcelaDto> CalcularPRICE(decimal valorPrincipal, decimal taxaJurosAnual, int numeroParcelas);

    /// <summary>
    /// Calcula ambos os sistemas de amortização (SAC e PRICE)
    /// </summary>
    /// <param name="valorPrincipal">Valor principal do empréstimo</param>
    /// <param name="taxaJurosAnual">Taxa de juros anual em formato decimal (ex: 0.0179 para 1.79% a.a.)</param>
    /// <param name="numeroParcelas">Número de parcelas mensais</param>
    List<ResultadoAmortizacaoDto> CalcularAmortizacoes(decimal valorPrincipal, decimal taxaJurosAnual, int numeroParcelas);
}
