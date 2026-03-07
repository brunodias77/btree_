namespace Catalog.Application.Features.Categories.Create;

public record CreateCategoryInput(
    string Name,
    string? Slug,
    Guid? ParentId,
    string? Description,
    string? ImageUrl,
    string? MetaTitle,
    string? MetaDescription,
    int SortOrder);
