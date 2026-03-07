using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Domain.Abstractions;

namespace Shared.Infrastructure.Data;

public static class DbContextExtensions
{
    /// <summary>
    /// Assembly do Shared.Infrastructure — carregado uma única vez para aplicar
    /// automaticamente as configurações de Outbox/Inbox em todos os módulos.
    /// </summary>
    private static readonly Assembly SharedInfraAssembly = typeof(DbContextExtensions).Assembly;

    /// <summary>
    /// Aplica configurações automáticas de modelagem:
    ///   - Schema padrão do módulo
    ///   - Configurações de Outbox/Inbox (shared.domain_events / shared.processed_events)
    ///   - Scan de IEntityTypeConfiguration do assembly do módulo
    ///   - Soft delete (Global Query Filter WHERE deleted_at IS NULL)
    ///   - Optimistic locking (Concurrency Token na coluna version)
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder do contexto.</param>
    /// <param name="moduleSchema">Schema do módulo (ex: "catalog", "users").</param>
    /// <param name="moduleAssembly">Assembly onde ficam as configurações do módulo.</param>
    public static void ApplyBaseModelConfigurations(
        this ModelBuilder modelBuilder,
        string moduleSchema,
        Assembly moduleAssembly)
    {
        modelBuilder.HasDefaultSchema(moduleSchema);

        // 1. Configurações compartilhadas (Outbox/Inbox) — sempre do assembly do Shared
        modelBuilder.ApplyConfigurationsFromAssembly(SharedInfraAssembly);

        // 2. Configurações específicas do módulo
        //    Só aplica se for um assembly diferente para evitar dupla aplicação
        if (moduleAssembly != SharedInfraAssembly)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(moduleAssembly);
        }

        // 3. Global Query Filters e Concurrency Tokens por convenção
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            // Soft Delete: global query filter WHERE deleted_at IS NULL
            if (typeof(ISoftDeletable).IsAssignableFrom(clrType))
            {
                var parameter = Expression.Parameter(clrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt));
                var nullConstant = Expression.Constant(null, typeof(DateTime?));
                var condition = Expression.Equal(property, nullConstant);
                var lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(clrType).HasQueryFilter(lambda);
            }

            // Optimistic Locking: Version como concurrency token
            if (typeof(IVersioned).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType)
                    .Property(nameof(IVersioned.Version))
                    .IsConcurrencyToken();
            }
        }
    }

    /// <summary>
    /// Aplica convenções automaticamente no ChangeTracker:
    ///   - CreatedAt/UpdatedAt timestamps
    ///   - Soft delete: converte Remove() em Update(DeletedAt = UtcNow)
    /// </summary>
    public static void ApplyBaseConventions(this ChangeTracker changeTracker)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in changeTracker.Entries())
        {
            // Soft Delete: converte EntityState.Deleted em Update com DeletedAt
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeletable softDeletable)
            {
                entry.State = EntityState.Modified;
                softDeletable.MarkAsDeleted();
            }

            // Timestamps
            if (entry.State == EntityState.Added)
            {
                var createdAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
                if (createdAt != null && createdAt.CurrentValue is DateTime dt && dt == default)
                {
                    createdAt.CurrentValue = now;
                }
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                var updatedAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
                if (updatedAt != null)
                {
                    updatedAt.CurrentValue = now;
                }
            }
        }
    }
}