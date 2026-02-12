using BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BuildingBlocks.Infrastructure.Persistence.QueryFilters;

/// <summary>
/// Filtro global de soft delete para EF Core.
/// Aplica automaticamente WHERE deleted_at IS NULL em todas as queries.
/// Mapeado para colunas 'deleted_at TIMESTAMPTZ' do schema.
/// </summary>
public static class SoftDeleteQueryFilter
{
    /// <summary>
    /// Aplica filtro de soft delete a um entity type builder.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade.</typeparam>
    /// <typeparam name="TId">Tipo do identificador.</typeparam>
    /// <param name="builder">Entity type builder.</param>
    public static void ApplyFilter<TEntity, TId>(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity<TId>
        where TId : notnull
    {
        builder.HasQueryFilter(e => e.DeletedAt == null);
    }

    /// <summary>
    /// Aplica filtros de soft delete para todas as entidades no ModelBuilder.
    /// </summary>
    /// <param name="modelBuilder">Model builder do EF Core.</param>
    public static void ApplyToAllEntities(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (IsSoftDeletable(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, "DeletedAt");
                var nullConstant = Expression.Constant(null, typeof(DateTime?));
                var comparison = Expression.Equal(property, nullConstant);
                var lambda = Expression.Lambda(comparison, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    /// <summary>
    /// Verifica se um tipo é soft deletable (herda de Entity<>).
    /// </summary>
    private static bool IsSoftDeletable(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == typeof(Entity<>))
            {
                return true;
            }
            baseType = baseType.BaseType;
        }
        return false;
    }

    /// <summary>
    /// Ignora o filtro de soft delete para uma query específica.
    /// Útil para queries administrativas ou de auditoria.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade.</typeparam>
    /// <param name="query">Query original.</param>
    /// <returns>Query sem filtro de soft delete.</returns>
    public static IQueryable<TEntity> IncludeDeleted<TEntity>(this IQueryable<TEntity> query)
        where TEntity : class
    {
        return query.IgnoreQueryFilters();
    }
}
