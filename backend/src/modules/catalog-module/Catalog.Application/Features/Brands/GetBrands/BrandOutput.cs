namespace Catalog.Application.Features.Brands.GetBrands;

public record BrandOutput(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? LogoUrl,
    bool IsActive,
    DateTimeOffset CreatedAt);
