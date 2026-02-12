using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.Middleware;

/// <summary>
/// Middleware para logging de requisições HTTP.
/// Registra informações de entrada, saída e tempo de resposta.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Ignora health checks e swagger
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var correlationId = context.GetCorrelationId();
        var stopwatch = Stopwatch.StartNew();

        // Log de entrada
        _logger.LogInformation(
            "Requisição iniciada: {Method} {Path} [CorrelationId: {CorrelationId}]",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log de saída
            var level = context.Response.StatusCode >= 400
                ? LogLevel.Warning
                : LogLevel.Information;

            _logger.Log(
                level,
                "Requisição finalizada: {Method} {Path} => {StatusCode} em {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);
        }
    }

    private static bool ShouldSkipLogging(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant() ?? string.Empty;

        return pathValue.Contains("/health") ||
               pathValue.Contains("/swagger") ||
               pathValue.Contains("/favicon");
    }
}
