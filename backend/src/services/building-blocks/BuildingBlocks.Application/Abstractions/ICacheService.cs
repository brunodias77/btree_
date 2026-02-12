namespace BuildingBlocks.Application.Abstractions;

/// <summary>
/// Interface para serviço de cache.
/// Usado para materialized views e otimização de consultas frequentes.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Obtém um valor do cache.
    /// </summary>
    /// <typeparam name="T">Tipo do valor.</typeparam>
    /// <param name="key">Chave do cache.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Valor do cache ou null se não encontrado.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Define um valor no cache com expiração opcional.
    /// </summary>
    /// <typeparam name="T">Tipo do valor.</typeparam>
    /// <param name="key">Chave do cache.</param>
    /// <param name="value">Valor a ser armazenado.</param>
    /// <param name="expiration">Tempo de expiração (opcional).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Remove um valor do cache.
    /// </summary>
    /// <param name="key">Chave do cache.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove todos os valores com prefixo especificado.
    /// Útil para invalidar cache de um domínio inteiro.
    /// </summary>
    /// <param name="prefix">Prefixo das chaves.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ou cria um valor no cache.
    /// Se o valor não existir, executa a factory e armazena o resultado.
    /// </summary>
    /// <typeparam name="T">Tipo do valor.</typeparam>
    /// <param name="key">Chave do cache.</param>
    /// <param name="factory">Função para criar o valor se não existir.</param>
    /// <param name="expiration">Tempo de expiração (opcional).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Valor do cache ou recém-criado.</returns>
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Verifica se uma chave existe no cache.
    /// </summary>
    /// <param name="key">Chave do cache.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se a chave existe.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza o tempo de expiração de uma chave.
    /// </summary>
    /// <param name="key">Chave do cache.</param>
    /// <param name="expiration">Novo tempo de expiração.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task RefreshAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
}
