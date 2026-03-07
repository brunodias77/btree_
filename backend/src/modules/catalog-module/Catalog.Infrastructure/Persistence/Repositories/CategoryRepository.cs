using Shared.Infrastructure.Data;
using Catalog.Domain.Aggregates.Category;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class CategoryRepository : Repository<Category, Guid, CatalogDbContext>, ICategoryRepository
{
    public CategoryRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(c => c.ParentId == categoryId, cancellationToken);
    }

    public async Task<bool> HasProductsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        // Precisamos acessar o DbSet de Products, que não está disponível diretamente no Repository genérico de Category.
        // Convertemos o Context para CatalogDbContext para acessar.
        var context = (CatalogDbContext)Context;
        return await context.Products.AnyAsync(p => p.CategoryId == categoryId, cancellationToken);
    }

    public async Task<bool> ExistsBySlugExcludingAsync(string slug, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(c => c.Slug == slug && c.Id != excludeId, cancellationToken);
    }

    public async Task<bool> IsDescendantOfAsync(Guid categoryId, Guid potentialAncestorId, CancellationToken cancellationToken = default)
    {
        // Get the category we want to check
        var category = await DbSet.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
        if (category == null) return false;

        // Get the potential ancestor
        var ancestor = await DbSet.FirstOrDefaultAsync(c => c.Id == potentialAncestorId, cancellationToken);
        if (ancestor == null) return false;

        // Check if the category's path starts with the ancestor's path
        // This means category is a descendant of ancestor
        if (category.Path == null || ancestor.Path == null) return false;
        
        return category.Path.StartsWith(ancestor.Path + "/") || category.Path == ancestor.Path;
    }
}

