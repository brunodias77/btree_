using Shared.Infrastructure.Data;
using Catalog.Domain.Aggregates.ProductReview;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class ProductReviewRepository : Repository<ProductReview, Guid, CatalogDbContext>, IProductReviewRepository
{
    public ProductReviewRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(r => r.UserId == userId && r.ProductId == productId, cancellationToken);
    }

    public async Task<double> GetAverageRatingAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var ratings = await DbSet
            .Where(r => r.ProductId == productId) // Considerar filtrar por status Approved?
            .Select(r => r.Rating)
            .ToListAsync(cancellationToken);

        if (!ratings.Any()) return 0;

        return ratings.Average();
    }

    public async Task<int> GetReviewCountAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(r => r.ProductId == productId, cancellationToken);
    }
}

