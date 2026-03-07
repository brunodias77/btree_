using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Web.Middlewares;

/// <summary>
/// Loga informações estruturadas de cada request HTTP.
/// Inclui método, path, status code, duração e correlation ID.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var correlationId = context.Items["CorrelationId"]?.ToString();
            var statusCode = context.Response.StatusCode;
            var method = context.Request.Method;
            var path = context.Request.Path;
            var elapsed = stopwatch.ElapsedMilliseconds;

            if (statusCode >= 500)
            {
                _logger.LogError(
                    "HTTP {Method} {Path} → {StatusCode} em {Elapsed}ms [CorrelationId: {CorrelationId}]",
                    method, path, statusCode, elapsed, correlationId);
            }
            else if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "HTTP {Method} {Path} → {StatusCode} em {Elapsed}ms [CorrelationId: {CorrelationId}]",
                    method, path, statusCode, elapsed, correlationId);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} → {StatusCode} em {Elapsed}ms [CorrelationId: {CorrelationId}]",
                    method, path, statusCode, elapsed, correlationId);
            }
        }
    }
}