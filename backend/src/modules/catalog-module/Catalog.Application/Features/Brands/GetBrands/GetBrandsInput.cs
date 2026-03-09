using Shared.Application.Models;

namespace Catalog.Application.Features.Brands.GetBrands;

public record GetBrandsInput : PagedRequest
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}
