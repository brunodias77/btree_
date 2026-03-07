using Microsoft.AspNetCore.Http;

namespace Shared.Web.Middlewares;

/// <summary>
/// Gera e propaga um Correlation ID para rastreamento de requests.
/// Se o cliente enviar X-Correlation-Id, reutiliza; caso contrário, gera um novo.
/// O ID é disponibilizado no HttpContext.Items e retornado no header da response.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Reutiliza o correlation ID do request ou gera um novo
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
            ?? Guid.NewGuid().ToString("N");

        // Disponibiliza para os middlewares e serviços downstream
        context.Items["CorrelationId"] = correlationId;

        // Adiciona ao header da response
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}