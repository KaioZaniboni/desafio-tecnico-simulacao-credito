using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace SimulacaoCredito.Infrastructure.Middleware;

/// <summary>
/// Middleware para tratamento global de exceções conforme especificado no documento de arquitetura
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlerMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log estruturado do erro para monitoramento interno
        _logger.LogError(exception,
            "Erro não tratado capturado pelo Exception Handler. " +
            "RequestPath: {RequestPath}, Method: {Method}, StatusCode: {StatusCode}, " +
            "ExceptionType: {ExceptionType}, Message: {Message}",
            context.Request.Path,
            context.Request.Method,
            context.Response.StatusCode,
            exception.GetType().Name,
            exception.Message);

        // Determina o status code e mensagem baseado no tipo de exceção
        var (statusCode, title, detail) = GetErrorResponse(exception);

        // Cria o Problem Details conforme RFC 7807
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = (int)statusCode,
            Instance = context.Request.Path,
            Type = GetProblemType(statusCode)
        };

        // Adiciona informações extras apenas em desenvolvimento
        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions.Add("exceptionType", exception.GetType().Name);
            problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
        }

        // Configura a resposta HTTP
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        // Serializa e retorna a resposta
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private (HttpStatusCode statusCode, string title, string detail) GetErrorResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (
                HttpStatusCode.BadRequest,
                "Parâmetro obrigatório",
                "Um parâmetro obrigatório não foi fornecido."
            ),
            ArgumentException => (
                HttpStatusCode.BadRequest,
                "Parâmetros inválidos",
                "Um ou mais parâmetros fornecidos são inválidos."
            ),
            InvalidOperationException => (
                HttpStatusCode.BadRequest,
                "Operação inválida",
                "A operação solicitada não pode ser executada no estado atual."
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Acesso negado",
                "Você não tem permissão para acessar este recurso."
            ),
            NotImplementedException => (
                HttpStatusCode.NotImplemented,
                "Funcionalidade não implementada",
                "Esta funcionalidade ainda não foi implementada."
            ),
            TimeoutException => (
                HttpStatusCode.RequestTimeout,
                "Tempo limite excedido",
                "A operação excedeu o tempo limite permitido."
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Erro interno do servidor",
                _environment.IsDevelopment() 
                    ? exception.Message 
                    : "Ocorreu um erro inesperado. Tente novamente mais tarde."
            )
        };
    }

    private static string GetProblemType(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            HttpStatusCode.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            HttpStatusCode.RequestTimeout => "https://tools.ietf.org/html/rfc7231#section-6.5.7",
            HttpStatusCode.NotImplemented => "https://tools.ietf.org/html/rfc7231#section-6.6.2",
            HttpStatusCode.InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _ => "https://tools.ietf.org/html/rfc7231"
        };
    }
}
