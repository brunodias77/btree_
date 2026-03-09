using Catalog.Domain.Aggregates.Product;

namespace Catalog.Domain.Repositories;

public interface IProductReadRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetByBrandAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Shared.Application.Models.PagedResult<Product>> GetByFilterPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm, 
        Guid? categoryId, 
        Guid? brandId, 
        string? status, 
        string? orderBy, 
        string? orderDirection, 
        CancellationToken cancellationToken = default);
}
