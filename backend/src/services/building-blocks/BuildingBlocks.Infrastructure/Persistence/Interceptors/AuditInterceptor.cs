using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor para auditoria automática de entidades.
/// Define CreatedAt e UpdatedAt automaticamente.
/// Mapeado para colunas 'created_at' e 'updated_at' do schema.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuditInterceptor> _logger;

    public AuditInterceptor(
        ICurrentUserService currentUserService,
        ILogger<AuditInterceptor> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        var utcNow = DateTime.UtcNow;
        var userId = _currentUserService.UserId;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // Todas as entidades que herdam de Entity<> são auditáveis
            if (!IsAuditableEntity(entry.Entity.GetType()))
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    SetPropertyValue(entry, "CreatedAt", utcNow);
                    SetPropertyValue(entry, "UpdatedAt", utcNow); // Satisfies NOT NULL constraint
                    _logger.LogDebug(
                        "Entidade {EntityType} criada por usuário {UserId}",
                        entry.Entity.GetType().Name,
                        userId);
                    break;

                case EntityState.Modified:
                    SetPropertyValue(entry, "UpdatedAt", utcNow);
                    _logger.LogDebug(
                        "Entidade {EntityType} atualizada por usuário {UserId}",
                        entry.Entity.GetType().Name,
                        userId);
                    break;
            }
        }
    }

    private static bool IsAuditableEntity(Type type)
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

    private static void SetPropertyValue(EntityEntry entry, string propertyName, object value)
    {
        var property = entry.Property(propertyName);
        if (property is not null)
        {
            property.CurrentValue = value;
        }
    }
}
