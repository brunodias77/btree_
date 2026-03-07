using Catalog.Domain.Repositories;
using Shared.Infrastructure.Data;

namespace Catalog.Infrastructure.Persistence;

/// <summary>
/// Unit of Work específico do módulo de Catálogo.
/// </summary>
public sealed class CatalogUnitOfWork : UnitOfWork<CatalogDbContext>, ICatalogUnitOfWork
{
    public CatalogUnitOfWork(CatalogDbContext context) : base(context)
    {
    }
}
