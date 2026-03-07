using Catalog.Domain.Aggregates.Category;
using Shared.Application.Data;

namespace Catalog.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugExcludingAsync(string slug, Guid excludeId, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<bool> IsDescendantOfAsync(Guid categoryId, Guid potentialAncestorId, CancellationToken cancellationToken = default);
}
