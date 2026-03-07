namespace Shared.Application.Abstractions;

/// <summary>
/// Interface base do serviço de cache.
/// Usado para materialized views e otimização de consultas frequentes.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Obtém um valor do cache.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Define um valor no cache com expiração opcional.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Remove um valor do cache.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove todos os valores cujas chaves iniciam com o prefixo especificado.
    /// Útil para invalidar cache de um domínio inteiro.
    /// </summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ou cria um valor no cache.
    /// Se o valor não existir, executa a factory e armazena o resultado.
    /// </summary>
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Verifica se uma chave existe no cache.
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza o tempo de expiração de uma chave.
    /// </summary>
    Task RefreshAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
}