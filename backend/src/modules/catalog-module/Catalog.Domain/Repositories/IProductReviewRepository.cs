using Catalog.Domain.Aggregates.ProductReview;
using Shared.Application.Data;

namespace Catalog.Domain.Repositories;

public interface IProductReviewRepository : IRepository<ProductReview, Guid>
{
    Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<int> GetReviewCountAsync(Guid productId, CancellationToken cancellationToken = default);
}
