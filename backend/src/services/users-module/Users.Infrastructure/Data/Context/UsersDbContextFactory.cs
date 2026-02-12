
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Users.Infrastructure.Data.Context;

/// <summary>
/// Fábrica de tempo de design para o UsersDbContext.
/// Necessária para executar migrações do Entity Framework Core.
/// </summary>
public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        // Tenta obter a string de conexão de variáveis de ambiente ou usa um padrão para desenvolvimento
        // Atualizado para corresponder ao docker-compose (Porta 5438)
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                               ?? "Host=localhost;Port=5438;Database=btree_db;Username=btree;Password=btree";

        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new UsersDbContext(optionsBuilder.Options);
    }
}
