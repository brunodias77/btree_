
using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Infrastructure.Events.Outbox;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// DbContext base para todos os módulos.
/// Inclui configurações padrão para auditoria, soft delete e outbox.
/// </summary>
public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Mensagens de outbox para eventos de domínio.
    /// Mapeado para tabela shared.domain_events no schema.
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica configuração do Outbox
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

        // Aplica filtro global de soft delete para todas as entidades que herdam de Entity
        ApplySoftDeleteFilter(modelBuilder);
    }

    /// <summary>
    /// Aplica filtro global de soft delete.
    /// </summary>
    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Verifica se a entidade é descendente de Entity<>
            var baseType = entityType.ClrType.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == typeof(Entity<>))
                {
                    // Aplica filtro: WHERE deleted_at IS NULL
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.Property(parameter, nameof(Entity<object>.DeletedAt));
                    var nullConstant = System.Linq.Expressions.Expression.Constant(null, typeof(DateTime?));
                    var comparison = System.Linq.Expressions.Expression.Equal(property, nullConstant);
                    var lambda = System.Linq.Expressions.Expression.Lambda(comparison, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    break;
                }
                baseType = baseType.BaseType;
            }
        }
    }

    /// <summary>
    /// Configura interceptors para o DbContext.
    /// Deve ser chamado em OnConfiguring dos DbContexts filhos.
    /// </summary>
    protected static void ConfigureInterceptors(
        DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider)
    {
        optionsBuilder.AddInterceptors(
            serviceProvider.GetRequiredService<AuditInterceptor>(),
            serviceProvider.GetRequiredService<DomainEventInterceptor>(),
            serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
    }
}
