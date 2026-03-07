using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Shared.Web.Filters;

/// <summary>
/// Action filter para idempotência de requests POST/PUT/PATCH.
/// O cliente envia o header "Idempotency-Key" e o servidor garante que
/// a mesma operação não será executada mais de uma vez.
/// 
/// Uso: [ServiceFilter(typeof(IdempotencyFilter))] ou [TypeFilter(typeof(IdempotencyFilter))]
/// </summary>
public class IdempotencyFilter : IAsyncActionFilter
{
    private const string IdempotencyHeader = "Idempotency-Key";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    private readonly IMemoryCache _cache;
    private readonly ILogger<IdempotencyFilter> _logger;

    public IdempotencyFilter(IMemoryCache cache, ILogger<IdempotencyFilter> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;

        // Só aplica idempotência para métodos que modificam estado
        if (method is not ("POST" or "PUT" or "PATCH"))
        {
            await next();
            return;
        }

        // Verifica se o header Idempotency-Key foi enviado
        if (!context.HttpContext.Request.Headers.TryGetValue(IdempotencyHeader, out var idempotencyKey)
            || string.IsNullOrWhiteSpace(idempotencyKey))
        {
            await next();
            return;
        }

        var cacheKey = $"idempotency:{idempotencyKey}";

        // Verifica se já temos uma resposta cacheada para esta key
        if (_cache.TryGetValue(cacheKey, out CachedResponse? cachedResponse) && cachedResponse is not null)
        {
            _logger.LogInformation(
                "Request idempotente detectado. Idempotency-Key: {IdempotencyKey}. Retornando resposta cacheada.",
                idempotencyKey.ToString());

            context.HttpContext.Response.StatusCode = cachedResponse.StatusCode;
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(cachedResponse.Body);
            return;
        }

        // Executa a action
        var executedContext = await next();

        // Cacheia a resposta se a action retornou um resultado (sem exceção)
        if (executedContext.Exception is null && executedContext.Result is ObjectResult objectResult)
        {
            var responseBody = JsonSerializer.Serialize(objectResult.Value, JsonOptions);
            var statusCode = objectResult.StatusCode ?? 200;

            _cache.Set(cacheKey, new CachedResponse(statusCode, responseBody), CacheDuration);

            _logger.LogDebug(
                "Resposta cacheada para Idempotency-Key: {IdempotencyKey}",
                idempotencyKey.ToString());
        }
    }

    private sealed record CachedResponse(int StatusCode, string Body);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}