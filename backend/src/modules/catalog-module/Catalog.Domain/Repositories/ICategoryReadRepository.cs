using Catalog.Domain.Aggregates.Category;

namespace Catalog.Domain.Repositories;

public interface ICategoryReadRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetRootsAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Category> Items, int TotalCount)> BrowseAsync(string? searchTerm, Guid? parentId, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default);
}
