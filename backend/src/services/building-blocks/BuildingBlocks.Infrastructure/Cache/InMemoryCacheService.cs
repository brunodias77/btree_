using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Cache;

/// <summary>
/// Implementação in-memory do serviço de cache.
/// Útil para desenvolvimento e ambientes de instância única.
/// </summary>
public sealed class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<InMemoryCacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);

    public InMemoryCacheService(IMemoryCache cache, ILogger<InMemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Obtém um valor do cache.
    /// </summary>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class
    {
        var value = _cache.Get<T>(key);

        if (value is not null)
        {
            _logger.LogDebug("Cache hit para chave {Key}", key);
        }
        else
        {
            _logger.LogDebug("Cache miss para chave {Key}", key);
        }

        return Task.FromResult(value);
    }

    /// <summary>
    /// Define um valor no cache.
    /// </summary>
    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
        };

        _cache.Set(key, value, options);
        _logger.LogDebug("Cache set para chave {Key} com expiração {Expiration}", key, expiration ?? _defaultExpiration);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove um valor do cache.
    /// </summary>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        _logger.LogDebug("Cache remove para chave {Key}", key);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove valores por prefixo.
    /// Nota: IMemoryCache não suporta nativamente, então esta operação é limitada.
    /// </summary>
    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "RemoveByPrefixAsync não é suportado por IMemoryCache. " +
            "Considere usar DistributedCacheService para esta funcionalidade.");

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
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var exists = _cache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }

    /// <summary>
    /// Atualiza o tempo de expiração.
    /// Nota: IMemoryCache não suporta refresh de expiração sem recriar o item.
    /// </summary>
    public Task RefreshAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        // IMemoryCache não suporta refresh, então precisamos reobter e ressetar o valor
        if (_cache.TryGetValue(key, out var value) && value is not null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            _cache.Set(key, value, options);
        }

        return Task.CompletedTask;
    }
}
