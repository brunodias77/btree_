using Catalog.Domain.Aggregates.UserFavorite;
using Shared.Application.Data;

namespace Catalog.Domain.Repositories;

public interface IUserFavoriteRepository : IRepository<UserFavorite, Guid>
{
    Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    Task<UserFavorite?> GetAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
}
