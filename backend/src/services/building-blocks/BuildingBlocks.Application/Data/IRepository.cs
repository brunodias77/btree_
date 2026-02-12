namespace BuildingBlocks.Application.Data;
/// <summary>
/// Interface genérica para repositório de agregados.
/// Encapsula operações de persistência mantendo o domínio agnóstico de infraestrutura.
/// </summary>
/// <typeparam name="TEntity">Tipo do aggregate root.</typeparam>
/// <typeparam name="TId">Tipo do identificador.</typeparam>
public interface IRepository<TEntity, in TId>
    where TEntity : class
    where TId : notnull
{


    /// <summary>
    /// Obtém uma entidade pelo ID.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A entidade ou null se não encontrada.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém a primeira entidade que atende à especificação.
    /// </summary>
    /// <param name="specification">Especificação de consulta.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A entidade ou null se não encontrada.</returns>
    Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todas as entidades.
    /// Use com cuidado em tabelas grandes.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de entidades.</returns>
    Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista entidades que atendem à especificação.
    /// </summary>
    /// <param name="specification">Especificação de consulta.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de entidades.</returns>
    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta o total de entidades.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Total de entidades.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta entidades que atendem à especificação.
    /// </summary>
    /// <param name="specification">Especificação de consulta.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Total de entidades.</returns>
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe alguma entidade que atenda à especificação.
    /// </summary>
    /// <param name="specification">Especificação de consulta.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se existe.</returns>
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma nova entidade.
    /// </summary>
    /// <param name="entity">Entidade a ser adicionada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona múltiplas entidades.
    /// </summary>
    /// <param name="entities">Entidades a serem adicionadas.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade existente.
    /// </summary>
    /// <param name="entity">Entidade a ser atualizada.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Remove uma entidade.
    /// </summary>
    /// <param name="entity">Entidade a ser removida.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Atalho para UnitOfWork.SaveChangesAsync().
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Número de entidades afetadas.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
