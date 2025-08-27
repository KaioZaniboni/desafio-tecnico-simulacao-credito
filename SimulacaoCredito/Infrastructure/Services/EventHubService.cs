using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using SimulacaoCredito.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace SimulacaoCredito.Infrastructure.Services;

public class EventHubService : IEventHubService, IDisposable
{
    private readonly EventHubProducerClient? _producerClient;
    private readonly ILogger<EventHubService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public EventHubService(IConfiguration configuration, ILogger<EventHubService> logger)
    {
        _logger = logger;

        var connectionString = configuration.GetConnectionString("EventHub");
        var entityPath = configuration["EventHub:EntityPath"];

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(entityPath))
        {
            _producerClient = null;
        }
        else
        {
            _producerClient = new EventHubProducerClient(connectionString, entityPath);
        }

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
        if (_producerClient == null)
        {
            return;
        }

        try
        {
            var json = JsonSerializer.Serialize(dados, _jsonOptions);
            var eventData = new EventData(Encoding.UTF8.GetBytes(json));

            eventData.Properties.Add("TipoEvento", tipoEvento);
            eventData.Properties.Add("Timestamp", DateTimeOffset.UtcNow.ToString("O"));
            eventData.Properties.Add("Source", "SimulacaoCredito");

            using var eventBatch = await _producerClient.CreateBatchAsync();

            if (!eventBatch.TryAdd(eventData))
            {
                throw new InvalidOperationException("Evento muito grande para ser adicionado ao batch");
            }

            await _producerClient.SendAsync(eventBatch);
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
