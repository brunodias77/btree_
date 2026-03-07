using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Web.Models;

namespace Shared.Web.Middlewares;

/// <summary>
/// Captura exceções não tratadas e retorna uma resposta JSON padronizada.
/// Em desenvolvimento, inclui detalhes da exceção; em produção, retorna mensagens genéricas.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
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
        // Se a response já começou a ser enviada, não podemos mais escrever
        if (context.Response.HasStarted)
        {
            _logger.LogWarning(exception,
                "Response já iniciada, não é possível escrever erro para {Path}",
                context.Request.Path);
            return;
        }

        var correlationId = context.Items["CorrelationId"]?.ToString();
        var isDev = _environment.IsDevelopment();

        var (statusCode, errorResponse) = exception switch
        {
            ArgumentException => (400, ApiErrorResponse.BadRequest("BadRequest",
                isDev ? exception.Message : "Dados inválidos.")),

            KeyNotFoundException => (404, ApiErrorResponse.NotFound(
                isDev ? exception.Message : "Recurso não encontrado.")),

            UnauthorizedAccessException => (401, ApiErrorResponse.Unauthorized(
                isDev ? exception.Message : "Não autorizado.")),

            InvalidOperationException => (409, ApiErrorResponse.Conflict(
                isDev ? exception.Message : "Operação conflitante.")),

            _ => (500, ApiErrorResponse.InternalError(
                isDev ? exception.Message : "Ocorreu um erro interno no servidor."))
        };

        _logger.LogError(exception,
            "Exceção não tratada [{StatusCode}] [CorrelationId: {CorrelationId}] {Path}: {Message}",
            statusCode, correlationId, context.Request.Path, exception.Message);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(errorResponse.ToJson());
    }
}