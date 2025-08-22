using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using SimulacaoCredito.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace SimulacaoCredito.Infrastructure.Services;

public class EventHubService : IEventHubService, IDisposable
{
    private readonly EventHubProducerClient _producerClient;
    private readonly ILogger<EventHubService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public EventHubService(IConfiguration configuration, ILogger<EventHubService> logger)
    {
        _logger = logger;
        
        var connectionString = configuration.GetConnectionString("EventHub") 
            ?? throw new InvalidOperationException("EventHub connection string não configurada");
        
        var entityPath = configuration["EventHub:EntityPath"] 
            ?? throw new InvalidOperationException("EventHub EntityPath não configurado");

        _producerClient = new EventHubProducerClient(connectionString, entityPath);
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task PublicarSimulacaoCriadaAsync(long simulacaoId, decimal valorDesejado, int prazo, int codigoProduto)
    {
        var evento = new
        {
            TipoEvento = "SimulacaoCriada",
            Timestamp = DateTimeOffset.UtcNow,
            SimulacaoId = simulacaoId,
            ValorDesejado = valorDesejado,
            Prazo = prazo,
            CodigoProduto = codigoProduto
        };

        await PublicarEventoAsync("SimulacaoCriada", evento);
    }

    public async Task PublicarEventoAsync(string tipoEvento, object dados)
    {
        try
        {
            var json = JsonSerializer.Serialize(dados, _jsonOptions);
            var eventData = new EventData(Encoding.UTF8.GetBytes(json));
            
            // Adicionar propriedades ao evento
            eventData.Properties.Add("TipoEvento", tipoEvento);
            eventData.Properties.Add("Timestamp", DateTimeOffset.UtcNow.ToString("O"));
            eventData.Properties.Add("Source", "SimulacaoCredito");

            using var eventBatch = await _producerClient.CreateBatchAsync();
            
            if (!eventBatch.TryAdd(eventData))
            {
                throw new InvalidOperationException("Evento muito grande para ser adicionado ao batch");
            }

            await _producerClient.SendAsync(eventBatch);
            
            _logger.LogInformation("Evento {TipoEvento} publicado com sucesso no EventHub", tipoEvento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar evento {TipoEvento} no EventHub", tipoEvento);
            throw;
        }
    }

    public void Dispose()
    {
        _producerClient?.DisposeAsync().AsTask().Wait();
    }
}
