using Catalog.Domain.Aggregates.Brand;
using Shared.Application.Data;

namespace Catalog.Domain.Repositories;

public interface IBrandRepository : IRepository<Brand, Guid>
{
    Task<Brand?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugExcludingAsync(string slug, Guid excludeId, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Brand> Items, int TotalCount)> BrowseAsync(
        string? searchTerm, bool? isActive, int page, int pageSize,
        CancellationToken cancellationToken = default);
}
