using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Domain.Entities;
using SimulacaoCredito.Infrastructure.Data;

namespace SimulacaoCredito.Infrastructure.Services;

public class TelemetriaService : ITelemetriaService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TelemetriaService> _logger;

    public TelemetriaService(AppDbContext context, ILogger<TelemetriaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task RegistrarRequisicaoAsync(string nomeApi, int tempoResposta, bool sucesso)
    {
        try
        {
            var telemetria = new TelemetriaRequisicao
            {
                NomeApi = nomeApi,
                DataHora = DateTime.UtcNow,
                TempoResposta = tempoResposta,
                Sucesso = sucesso,
                StatusCode = sucesso ? 200 : 500
            };

            _context.Set<TelemetriaRequisicao>().Add(telemetria);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar telemetria para {NomeApi}", nomeApi);
        }
    }

    public async Task<TelemetriaResponseDto> ObterTelemetriaDiaAsync(DateTime dataReferencia)
    {
        var dataInicio = dataReferencia.Date;
        var dataFim = dataInicio.AddDays(1);

        var telemetriaDia = await _context.Set<TelemetriaRequisicao>()
            .Where(t => t.DataHora >= dataInicio && t.DataHora < dataFim)
            .GroupBy(t => t.NomeApi)
            .Select(g => new TelemetriaEndpointDto
            {
                NomeApi = g.Key,
                QtdRequisicoes = g.Count(),
                TempoMedio = (int)g.Average(t => t.TempoResposta),
                TempoMinimo = g.Min(t => t.TempoResposta),
                TempoMaximo = g.Max(t => t.TempoResposta),
                PercentualSucesso = Math.Round((decimal)g.Count(t => t.Sucesso) / g.Count() * 100, 2)
            })
            .ToListAsync();

        return new TelemetriaResponseDto
        {
            DataReferencia = dataReferencia.ToString("yyyy-MM-dd"),
            ListaEndpoints = telemetriaDia
        };
    }
}
