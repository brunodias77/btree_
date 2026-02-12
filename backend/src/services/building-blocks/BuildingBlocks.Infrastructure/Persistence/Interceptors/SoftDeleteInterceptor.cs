using BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor para soft delete automático.
/// Converte DELETE em UPDATE SET deleted_at = NOW().
/// Mapeado para coluna 'deleted_at TIMESTAMPTZ' do schema.
/// </summary>
public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<SoftDeleteInterceptor> _logger;

    public SoftDeleteInterceptor(ILogger<SoftDeleteInterceptor> logger)
    {
        _logger = logger;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ConvertDeleteToSoftDelete(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ConvertDeleteToSoftDelete(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Converte operações de DELETE em soft delete (UPDATE deleted_at).
    /// </summary>
    private void ConvertDeleteToSoftDelete(DbContext context)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Deleted)
                continue;

            // Verifica se é uma entidade soft-deletable (herda de Entity<>)
            if (!IsSoftDeletable(entry.Entity.GetType()))
                continue;

            // Converte DELETE para UPDATE
            entry.State = EntityState.Modified;

            // Define DeletedAt
            var deletedAtProperty = entry.Property("DeletedAt");
            if (deletedAtProperty is not null)
            {
                deletedAtProperty.CurrentValue = utcNow;
            }

            _logger.LogDebug(
                "Soft delete aplicado para {EntityType} com ID {EntityId}",
                entry.Entity.GetType().Name,
                GetEntityId(entry.Entity));
        }
    }

    /// <summary>
    /// Verifica se um tipo é soft deletable.
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
    /// Obtém o ID da entidade para logging.
    /// </summary>
    private static object? GetEntityId(object entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        return idProperty?.GetValue(entity);
    }
}
