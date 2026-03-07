using Catalog.Domain.Aggregates.Brand;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class BrandRepository :  Repository<Brand, Guid, CatalogDbContext>, IBrandRepository
{
    public BrandRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<Brand?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(b => b.Slug == slug, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(b => b.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(b => b.Slug == slug, cancellationToken);
    }

    public async Task<bool> ExistsBySlugExcludingAsync(string slug, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(b => b.Slug == slug && b.Id != excludeId, cancellationToken);
    }

    public async Task<bool> HasProductsAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        var context = (CatalogDbContext)Context;
        return await context.Products.AnyAsync(p => p.BrandId == brandId, cancellationToken);
    }

    public async Task<(IReadOnlyList<Brand> Items, int TotalCount)> BrowseAsync(
        string? searchTerm, bool? isActive, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(b => !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(b => b.Name.ToLower().Contains(term));
        }

        if (isActive.HasValue)
        {
            query = query.Where(b => b.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public override async Task<IReadOnlyList<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }
}
