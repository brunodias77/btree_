using Catalog.Domain.Aggregates.StockReservation;
using Shared.Application.Data;

namespace Catalog.Domain.Repositories;

public interface IStockReservationRepository : IRepository<StockReservation, Guid>
{
    Task<int> GetReservedStockAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<StockReservation?> GetByReferenceAsync(string referenceType, Guid referenceId, Guid productId, CancellationToken cancellationToken = default);
    Task<List<StockReservation>> GetExpiredReservationsAsync(DateTime expirationDate, CancellationToken cancellationToken = default);
}
