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
            await _next(context);

            stopwatch.Stop();
            var sucesso = context.Response.StatusCode >= 200 && context.Response.StatusCode < 400;

            await telemetriaService.RegistrarRequisicaoAsync(nomeApi, (int)stopwatch.ElapsedMilliseconds, sucesso);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            await telemetriaService.RegistrarRequisicaoAsync(nomeApi, (int)stopwatch.ElapsedMilliseconds, false);

            _logger.LogError(ex, "Erro na requisição {NomeApi}", nomeApi);

            throw;
        }
    }
}
