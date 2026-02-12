using System.Net;
using System.Text.Json;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.Middleware;

/// <summary>
/// Middleware para tratamento global de exceções.
/// Converte exceções em respostas HTTP padronizadas.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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
        var correlationId = context.GetCorrelationId() ?? Guid.NewGuid().ToString("N");

        // Log da exceção
        _logger.LogError(
            exception,
            "Erro não tratado na requisição {Method} {Path} [CorrelationId: {CorrelationId}]",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        // Determina status code e mensagem baseado no tipo de exceção
        var (statusCode, errorResponse) = exception switch
        {
            EntityNotFoundException e => (
                HttpStatusCode.NotFound,
                ApiErrorResponse.NotFound(e.Message, correlationId)),

            BusinessRuleException e => (
                HttpStatusCode.BadRequest,
                ApiErrorResponse.BadRequest(e.Code, e.Message, correlationId)),

            DomainValidationException e => (
                HttpStatusCode.BadRequest,
                ApiErrorResponse.ValidationError(e.Message, correlationId)),

            ConcurrencyException e => (
                HttpStatusCode.Conflict,
                ApiErrorResponse.Conflict(e.Message, correlationId)),

            DomainException e => (
                HttpStatusCode.BadRequest,
                ApiErrorResponse.BadRequest(e.Code, e.Message, correlationId)),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                ApiErrorResponse.Unauthorized("Não autorizado.", correlationId)),

            ArgumentException e => (
                HttpStatusCode.BadRequest,
                ApiErrorResponse.ValidationError(e.Message, correlationId)),

            InvalidOperationException e when e.Message.Contains("JWT") => (
                HttpStatusCode.Unauthorized,
                ApiErrorResponse.Unauthorized("Token inválido ou expirado.", correlationId)),

            _ => (
                HttpStatusCode.InternalServerError,
                CreateInternalErrorResponse(exception, correlationId))
        };

        // Resposta
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(errorResponse, JsonOptions);
        await context.Response.WriteAsync(json);
    }

    private ApiErrorResponse CreateInternalErrorResponse(Exception exception, string correlationId)
    {
        // Em desenvolvimento, mostra detalhes do erro
        if (_environment.IsDevelopment())
        {
            return new ApiErrorResponse
            {
                Success = false,
                Error = new ErrorDetails
                {
                    Code = "InternalError",
                    Message = exception.Message,
                    Details = exception.StackTrace
                },
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow
            };
        }

        // Em produção, mensagem genérica
        return ApiErrorResponse.InternalError(correlationId);
    }
}
