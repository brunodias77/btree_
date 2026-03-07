using Microsoft.EntityFrameworkCore;
using Users.Infrastructure.Data.Context;

namespace Btree.Api.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Applying migrations for UsersDbContext...");
            var usersContext = services.GetRequiredService<UsersDbContext>();
            await usersContext.Database.MigrateAsync();
            logger.LogInformation("UsersDbContext migrations applied successfully.");

            logger.LogInformation("Applying migrations for CatalogDbContext...");
            var catalogContext = services.GetRequiredService<Catalog.Infrastructure.Persistence.CatalogDbContext>();
            await catalogContext.Database.MigrateAsync();
            logger.LogInformation("CatalogDbContext migrations applied successfully.");
            
            // No futuro, podemos injetar uma lista de DbContexts de outros módulos aqui 
            // ou registrar a aplicação de migrations de cada módulo iterativamente.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }
}
