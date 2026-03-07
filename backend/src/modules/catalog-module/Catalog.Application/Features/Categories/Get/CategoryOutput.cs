namespace Catalog.Application.Features.Categories.Get;

public record CategoryOutput(
    Guid Id,
    string Name,
    string Slug,
    Guid? ParentId,
    string? Path,
    bool IsActive,
    DateTimeOffset CreatedAt);
