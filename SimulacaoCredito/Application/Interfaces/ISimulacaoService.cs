using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Domain.Entities;

namespace SimulacaoCredito.Application.Interfaces;

public interface ISimulacaoService
{
    /// <summary>
    /// Cria uma nova simulação de crédito
    /// </summary>
    Task<SimulacaoResponseDto> CriarSimulacaoAsync(SimulacaoRequestDto request);
    
    /// <summary>
    /// Obtém uma simulação por ID
    /// </summary>
    Task<SimulacaoResponseDto?> ObterSimulacaoPorIdAsync(long id);
    
    /// <summary>
    /// Lista simulações com paginação
    /// </summary>
    Task<ListaSimulacoesResponseDto> ListarSimulacoesAsync(int pagina = 1, int tamanhoPagina = 10);
    
    /// <summary>
    /// Obtém volume de simulações por produto e dia
    /// </summary>
    Task<VolumePorProdutoDiaResponseDto> ObterVolumePorProdutoDiaAsync(DateTime dataReferencia);
}
