namespace Catalog.Application.Features.Brands.Commands.UpdateBrand;

public record UpdateBrandInput(
    Guid Id,
    string Name,
    string? Description = null,
    string? LogoUrl = null,
    bool IsActive = true);
