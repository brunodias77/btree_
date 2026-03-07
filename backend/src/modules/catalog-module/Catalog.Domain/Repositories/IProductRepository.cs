using Catalog.Domain.Aggregates.Product;
using Shared.Application.Data;

namespace Catalog.Domain.Repositories;

public interface IProductRepository : IRepository<Product, Guid>
{
    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
}
