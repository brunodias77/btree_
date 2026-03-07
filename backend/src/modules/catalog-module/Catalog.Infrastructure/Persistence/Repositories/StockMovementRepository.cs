using Shared.Infrastructure.Data;
using Catalog.Domain.Aggregates.StockMovement;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class StockMovementRepository : Repository<StockMovement, Guid, CatalogDbContext>, IStockMovementRepository
{
    public StockMovementRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<int> GetCurrentStockAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var context = (CatalogDbContext)Context;
        // O estoque atual está na entidade Product, não calculamos via movimentos (snapshot)
        var product = await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
            
        return product?.Stock ?? 0;
    }
}

