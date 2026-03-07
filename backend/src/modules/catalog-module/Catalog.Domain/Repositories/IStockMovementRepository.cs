using Catalog.Domain.Aggregates.StockMovement;
using Shared.Application.Data;

namespace Catalog.Domain.Repositories;

public interface IStockMovementRepository : IRepository<StockMovement, Guid>
{
    Task<int> GetCurrentStockAsync(Guid productId, CancellationToken cancellationToken = default);
}
