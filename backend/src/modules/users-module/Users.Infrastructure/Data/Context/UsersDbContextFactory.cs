using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Users.Infrastructure.Data.Context;

/// <summary>
/// Factory para criação do UsersDbContext em design-time (migrations).
/// Usado por: dotnet ef migrations add ... --project Users.Infrastructure
/// </summary>
public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();

        // Connection string padrão para design-time (migrations)
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5438;Database=btree_db;Username=btree;Password=btree",
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "users");
            });

        return new UsersDbContext(optionsBuilder.Options);
    }
}