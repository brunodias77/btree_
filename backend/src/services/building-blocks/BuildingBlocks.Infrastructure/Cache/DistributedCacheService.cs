using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Cache;

/// <summary>
/// Implementação distribuída do serviço de cache.
/// Usa IDistributedCache (Redis, SQL Server, etc.) para ambientes multi-instância.
/// Ideal para materialized views e dados compartilhados entre instâncias.
/// </summary>
public sealed class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public DistributedCacheService(
        IDistributedCache cache,
        ILogger<DistributedCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Obtém um valor do cache.
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class
    {
        var bytes = await _cache.GetAsync(key, cancellationToken);

        if (bytes is null)
        {
            _logger.LogDebug("Cache miss para chave {Key}", key);
            return null;
        }

        _logger.LogDebug("Cache hit para chave {Key}", key);
        return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
    }

    /// <summary>
    /// Define um valor no cache.
    /// </summary>
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
        };

        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
        await _cache.SetAsync(key, bytes, options, cancellationToken);

        _logger.LogDebug(
            "Cache set para chave {Key} com expiração {Expiration}",
            key,
            expiration ?? _defaultExpiration);
    }

    /// <summary>
    /// Remove um valor do cache.
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
        _logger.LogDebug("Cache remove para chave {Key}", key);
    }

    /// <summary>
    /// Remove valores por prefixo.
    /// Nota: Requer implementação específica do provider (ex: Redis SCAN).
    /// Esta implementação básica apenas loga um aviso.
    /// </summary>
    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Nota: IDistributedCache não suporta nativamente busca por prefixo.
        // Para Redis, seria necessário usar ConnectionMultiplexer diretamente.
        _logger.LogWarning(
            "RemoveByPrefixAsync com prefixo {Prefix} requer implementação específica do provider.",
            prefix);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtém ou cria um valor no cache.
    /// </summary>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var value = await GetAsync<T>(key, cancellationToken);

        if (value is not null)
        {
            return value;
        }

        value = await factory(cancellationToken);
        await SetAsync(key, value, expiration, cancellationToken);

        return value;
    }

    /// <summary>
    /// Verifica se uma chave existe no cache.
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await _cache.GetAsync(key, cancellationToken);
        return bytes is not null;
    }

    /// <summary>
    /// Atualiza o tempo de expiração (refresh sliding expiration).
    /// </summary>
    public async Task RefreshAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        // Para fazer refresh real, precisamos obter o valor e re-setar com nova expiração
        var bytes = await _cache.GetAsync(key, cancellationToken);

        if (bytes is not null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            await _cache.SetAsync(key, bytes, options, cancellationToken);
        }
    }
}
