namespace BuildingBlocks.Application.Data;
/// <summary>
/// Interface para Unit of Work.
/// Gerencia transações e garante consistência em operações que envolvem múltiplos repositórios.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Persiste todas as alterações pendentes.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Número de entidades afetadas.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia uma nova transação.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a transação atual.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reverte a transação atual.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Indica se há uma transação ativa.
    /// </summary>
    bool HasActiveTransaction { get; }
}
