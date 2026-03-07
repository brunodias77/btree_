namespace Catalog.Application.Features.Categories.Update;

public record UpdateCategoryInput(
    Guid Id,
    string Name,
    string? Slug,
    string? Description,
    string? ImageUrl,
    string? MetaTitle,
    string? MetaDescription,
    int SortOrder);
