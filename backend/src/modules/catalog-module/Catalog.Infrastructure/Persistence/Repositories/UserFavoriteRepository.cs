using Shared.Infrastructure.Data;
using Catalog.Domain.Aggregates.UserFavorite;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class UserFavoriteRepository : Repository<UserFavorite, Guid, CatalogDbContext>, IUserFavoriteRepository
{
    public UserFavoriteRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(f => f.UserId == userId && f.ProductId == productId, cancellationToken);
    }

    public async Task<UserFavorite?> GetAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId, cancellationToken);
    }
}

