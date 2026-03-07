using Microsoft.EntityFrameworkCore;
using Shared.Outbox;

namespace Shared.Infrastructure.Data;

/// <summary>
/// DbContext base para todos os módulos do monolito modular.
/// Cada módulo deve herdar desta classe, sobrescrever <see cref="Schema"/>
/// e configurar apenas as suas próprias entidades.
///
/// Funcionalidades já incluídas (não precisam ser repetidas nos módulos):
///   - <see cref="OutboxMessages"/> → tabela shared.domain_events
///   - <see cref="InboxMessages"/>  → tabela shared.processed_events
///   - Atualização automática de timestamps (CreatedAt/UpdatedAt)
///   - Soft delete com Global Query Filter (WHERE deleted_at IS NULL)
///   - Optimistic locking via concurrency token (coluna version)
/// </summary>
public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Schema do módulo no banco (ex: "catalog", "orders", "payments").
    /// Cada módulo deve sobrescrever esta propriedade.
    /// </summary>
    protected abstract string Schema { get; }

    // ── Outbox / Inbox ────────────────────────────────────────────────────────
    // Declarados aqui uma única vez; as configurações de tabela ficam em
    // Shared.Infrastructure/Data/Configurations e são aplicadas automaticamente.

    /// <summary>Mensagens de domínio pendentes de publicação (Outbox Pattern).</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <summary>Eventos já processados — garante idempotência (Inbox Pattern).</summary>
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    // ─────────────────────────────────────────────────────────────────────────

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica: schema padrão + configs do Shared (Outbox/Inbox) +
        //         configs do módulo + soft-delete filters + version tokens
        modelBuilder.ApplyBaseModelConfigurations(Schema, GetType().Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.ApplyBaseConventions();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ChangeTracker.ApplyBaseConventions();
        return base.SaveChanges();
    }
}