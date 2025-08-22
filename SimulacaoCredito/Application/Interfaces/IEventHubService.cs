namespace SimulacaoCredito.Application.Interfaces;

public interface IEventHubService
{
    /// <summary>
    /// Publica evento de simulação criada
    /// </summary>
    Task PublicarSimulacaoCriadaAsync(long simulacaoId, decimal valorDesejado, int prazo, int codigoProduto);
    
    /// <summary>
    /// Publica evento genérico
    /// </summary>
    Task PublicarEventoAsync(string tipoEvento, object dados);
}
