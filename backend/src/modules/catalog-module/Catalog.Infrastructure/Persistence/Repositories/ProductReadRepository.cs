using Catalog.Domain.Aggregates.Product;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class ProductReadRepository : IProductReadRepository
{
    private readonly CatalogDbContext _context;

    public ProductReadRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Where(p => p.IsFeatured && p.Status == Domain.Enums.ProductStatus.Active)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetLatestAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Where(p => p.Status == Domain.Enums.ProductStatus.Active)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByBrandAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Where(p => p.BrandId == brandId && p.Status == Domain.Enums.ProductStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Where(p => p.CategoryId == categoryId && p.Status == Domain.Enums.ProductStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<Shared.Application.Models.PagedResult<Product>> GetByFilterPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm, 
        Guid? categoryId, 
        Guid? brandId, 
        string? status, 
        string? orderBy, 
        string? orderDirection, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _context.Products
            .AsNoTracking()
            .Include(p => p.Images.Where(i => i.IsPrimary)); // Include only primary image for listing

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Sku.Contains(searchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (brandId.HasValue)
        {
            query = query.Where(p => p.BrandId == brandId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Enums.ProductStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(p => p.Status == parsedStatus);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = (orderBy?.ToLowerInvariant(), orderDirection?.ToLowerInvariant()) switch
        {
            ("price", "asc") => query.OrderBy(p => p.Price),
            ("price", "desc") => query.OrderByDescending(p => p.Price),
            ("name", "asc") => query.OrderBy(p => p.Name),
            ("name", "desc") => query.OrderByDescending(p => p.Name),
            ("stock", "asc") => query.OrderBy(p => p.Stock),
            ("stock", "desc") => query.OrderByDescending(p => p.Stock),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new Shared.Application.Models.PagedResult<Product>(items, page, pageSize, totalCount);
    }
}
