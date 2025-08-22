using SimulacaoCredito.Application.Interfaces;
using System.Diagnostics;

namespace SimulacaoCredito.Infrastructure.Middleware;

public class TelemetriaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TelemetriaMiddleware> _logger;

    public TelemetriaMiddleware(RequestDelegate next, ILogger<TelemetriaMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITelemetriaService telemetriaService)
    {
        var stopwatch = Stopwatch.StartNew();
        var nomeApi = $"{context.Request.Method} {context.Request.Path}";
        var enderecoIp = context.Connection.RemoteIpAddress?.ToString();
        
        try
        {
            // Log de início da requisição
            _logger.LogInformation("Iniciando requisição {NomeApi} de {IP}", nomeApi, enderecoIp);

            await _next(context);
            
            stopwatch.Stop();
            var sucesso = context.Response.StatusCode >= 200 && context.Response.StatusCode < 400;
            
            // Registrar telemetria
            await telemetriaService.RegistrarRequisicaoAsync(nomeApi, (int)stopwatch.ElapsedMilliseconds, sucesso);
            
            // Log de fim da requisição
            _logger.LogInformation("Requisição {NomeApi} finalizada em {Tempo}ms com status {Status}", 
                nomeApi, stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Registrar telemetria de erro
            await telemetriaService.RegistrarRequisicaoAsync(nomeApi, (int)stopwatch.ElapsedMilliseconds, false);
            
            // Log de erro
            _logger.LogError(ex, "Erro na requisição {NomeApi} após {Tempo}ms", nomeApi, stopwatch.ElapsedMilliseconds);
            
            // Re-throw para manter o comportamento padrão
            throw;
        }
    }
}
