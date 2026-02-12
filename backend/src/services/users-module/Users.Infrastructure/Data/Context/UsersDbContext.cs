using System.Reflection;
using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Infrastructure.Events.Outbox;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Aggregates.Addresses;
using Users.Domain.Aggregates.LoginHistory;
using Users.Domain.Aggregates.Notification;
using Users.Domain.Aggregates.NotificationPreference;
using Users.Domain.Aggregates.Profile;
using Users.Domain.Aggregates.Session;
using Users.Domain.Identity;

namespace Users.Infrastructure.Data.Context;

public sealed class UsersDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Define o schema padrão para "users"
        builder.HasDefaultSchema("users");

        // Aplica as configurações definidas no assembly atual (Users.Infrastructure)
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Aplica configuração do Outbox explicitamente (já que não herdamos de BaseDbContext)
        builder.ApplyConfiguration(new OutboxMessageConfiguration());

        // Customização dos nomes das tabelas do Identity para seguir o padrão snake_case do schema
        builder.Entity<ApplicationUser>(b => b.ToTable("asp_net_users"));
        builder.Entity<ApplicationRole>(b => b.ToTable("asp_net_roles"));
        builder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("asp_net_user_claims"));
        builder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable("asp_net_role_claims"));
        builder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable("asp_net_user_logins"));
        builder.Entity<IdentityUserToken<Guid>>(b => b.ToTable("asp_net_user_tokens"));
        builder.Entity<IdentityUserRole<Guid>>(b => b.ToTable("asp_net_user_roles"));

        // Aplica filtro global de Soft Delete
        ApplySoftDeleteFilter(builder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Interceptadors são configurados no DependencyInjection via AddDbContext
        // Mas se precisarmos forçar aqui, podemos usar métodos de extensão do BuildingBlocks
    }
    
    /// <summary>
    /// Aplica filtro global para ignorar registros marcados como deletados (Soft Delete).
    /// Réplica da lógica do BaseDbContext pois não podemos herdar dele e do IdentityDbContext ao mesmo tempo.
    /// </summary>
    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var baseType = entityType.ClrType.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == typeof(Entity<>))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.Property(parameter, nameof(Entity<Guid>.DeletedAt));
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
    
    
    
}