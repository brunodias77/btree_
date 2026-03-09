namespace Catalog.Application.Features.Brands.Commands.CreateBrand;

public record CreateBrandInput(
    string Name,
    string? Description = null,
    string? LogoUrl = null,
    bool IsActive = true);
