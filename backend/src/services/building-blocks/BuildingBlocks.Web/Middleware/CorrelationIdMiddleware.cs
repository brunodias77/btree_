using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.Middleware;

/// <summary>
/// Middleware para gerenciamento de Correlation ID.
/// Garante rastreabilidade de requisições entre serviços.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    /// <summary>
    /// Nome do header para Correlation ID.
    /// </summary>
    public const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Obtém ou gera Correlation ID
        var correlationId = GetOrCreateCorrelationId(context);

        // Adiciona ao response header
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        // Adiciona ao contexto para uso em logs
        context.Items["CorrelationId"] = correlationId;

        // Configura escopo de log com Correlation ID
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await _next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        // Tenta obter do header
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId) &&
            !string.IsNullOrEmpty(correlationId))
        {
            return correlationId.ToString();
        }

        // Gera novo ID
        return Guid.NewGuid().ToString("N");
    }
}

/// <summary>
/// Extensões para obter Correlation ID do HttpContext.
/// </summary>
public static class CorrelationIdExtensions
{
    /// <summary>
    /// Obtém o Correlation ID do contexto.
    /// </summary>
    public static string? GetCorrelationId(this HttpContext context)
    {
        return context.Items["CorrelationId"] as string;
    }
}
