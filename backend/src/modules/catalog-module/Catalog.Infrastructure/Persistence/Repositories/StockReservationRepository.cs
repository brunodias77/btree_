using Shared.Infrastructure.Data;
using Catalog.Domain.Aggregates.StockReservation;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class StockReservationRepository : Repository<StockReservation, Guid, CatalogDbContext>, IStockReservationRepository
{
    public StockReservationRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<int> GetReservedStockAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.ProductId == productId && r.ExpiresAt > DateTime.UtcNow && r.ReleasedAt == null)
            .SumAsync(r => r.Quantity, cancellationToken);
    }

    public async Task<StockReservation?> GetByReferenceAsync(string referenceType, Guid referenceId, Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(r => 
                r.ReferenceType == referenceType && 
                r.ReferenceId == referenceId && 
                r.ProductId == productId, 
                cancellationToken);
    }

    public async Task<List<StockReservation>> GetExpiredReservationsAsync(DateTime expirationDate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.ExpiresAt <= expirationDate && r.ReleasedAt == null)
            .ToListAsync(cancellationToken);
    }
}

