using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;
using Shared.Outbox;
using Users.Domain.Aggregates.Addresses;
using Users.Domain.Aggregates.LoginHistory;
using Users.Domain.Aggregates.Notifications;
using Users.Domain.Aggregates.NotificationsPreference;
using Users.Domain.Aggregates.Profiles;
using Users.Domain.Aggregates.Sessions;
using Users.Domain.Aggregates.Tokens;
using Users.Domain.Identity;

namespace Users.Infrastructure.Data.Context;

/// <summary>
/// DbContext do módulo de usuários.
/// Herda de IdentityDbContext (não de BaseDbContext), mas aplica as mesmas
/// convenções de Outbox/Inbox através de <see cref="DbContextExtensions.ApplyBaseModelConfigurations"/>.
///
/// OutboxMessages → shared.domain_events
/// InboxMessages  → shared.processed_events
/// </summary>
public class UsersDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private const string Schema = "users";

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    // ── Entidades do módulo ───────────────────────────────────────────────────

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<EmailConfirmationToken> EmailConfirmationTokens => Set<EmailConfirmationToken>();

    // ── Outbox / Inbox (herdados via convenção compartilhada) ─────────────────

    /// <summary>Mensagens de domínio pendentes de publicação (Outbox Pattern).</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <summary>Eventos já processados — garante idempotência (Inbox Pattern).</summary>
    internal DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    // ─────────────────────────────────────────────────────────────────────────

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Snake_case para tabelas do Identity
        modelBuilder.Entity<ApplicationUser>(b => b.ToTable("asp_net_users"));
        modelBuilder.Entity<ApplicationRole>(b => b.ToTable("asp_net_roles"));
        modelBuilder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("asp_net_user_claims"));
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable("asp_net_role_claims"));
        modelBuilder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable("asp_net_user_logins"));
        modelBuilder.Entity<IdentityUserToken<Guid>>(b => b.ToTable("asp_net_user_tokens"));
        modelBuilder.Entity<IdentityUserRole<Guid>>(b => b.ToTable("asp_net_user_roles"));

        // Aplica: schema "users" + configs Outbox/Inbox (shared) +
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