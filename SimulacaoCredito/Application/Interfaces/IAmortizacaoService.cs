using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Domain.Entities;

namespace SimulacaoCredito.Application.Interfaces;

public interface IAmortizacaoService
{
    /// <summary>
    /// Calcula as parcelas usando o Sistema de Amortização Constante (SAC)
    /// </summary>
    List<ParcelaDto> CalcularSAC(decimal valorPrincipal, decimal taxaJurosAnual, int numeroParcelas);
    
    /// <summary>
    /// Calcula as parcelas usando o Sistema Francês de Amortização (PRICE)
    /// </summary>
    List<ParcelaDto> CalcularPRICE(decimal valorPrincipal, decimal taxaJurosAnual, int numeroParcelas);
    
    /// <summary>
    /// Calcula ambos os sistemas de amortização
    /// </summary>
    List<ResultadoAmortizacaoDto> CalcularAmortizacoes(decimal valorPrincipal, decimal taxaJurosAnual, int numeroParcelas);
}
