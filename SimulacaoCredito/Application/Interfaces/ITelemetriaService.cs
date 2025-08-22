using SimulacaoCredito.Application.DTOs;

namespace SimulacaoCredito.Application.Interfaces;

public interface ITelemetriaService
{
    /// <summary>
    /// Registra uma requisição para telemetria
    /// </summary>
    Task RegistrarRequisicaoAsync(string nomeApi, int tempoResposta, bool sucesso);
    
    /// <summary>
    /// Obtém dados de telemetria para uma data específica
    /// </summary>
    Task<TelemetriaResponseDto> ObterTelemetriaDiaAsync(DateTime dataReferencia);
}
