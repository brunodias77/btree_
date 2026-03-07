using Microsoft.EntityFrameworkCore;
using Shared.Application.Data;
using Shared.Domain.Abstractions;

namespace Shared.Infrastructure.Data;

/// <summary>
/// Implementação base de repositório usando EF Core.
/// Cada módulo deve herdar desta classe com seu DbContext e entidade específicos.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade.</typeparam>
/// <typeparam name="TId">Tipo do identificador.</typeparam>
/// <typeparam name="TContext">Tipo do DbContext.</typeparam>
public abstract class Repository<TEntity, TId, TContext> : IRepository<TEntity, TId>
    where TEntity : class
    where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected Repository(TContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    /// <summary>
    /// Remove a entidade. Se implementar ISoftDeletable, o BaseDbContext
    /// intercepta o EntityState.Deleted e converte em soft delete automaticamente.
    /// </summary>
    public virtual void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}