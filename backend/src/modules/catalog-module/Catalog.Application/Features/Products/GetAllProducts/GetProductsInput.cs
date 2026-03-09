using Shared.Application.Models;

namespace Catalog.Application.Features.Products.GetAllProducts;

public record GetProductsInput : PagedRequest
{
    public string? SearchTerm { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? BrandId { get; init; }
    public string? Status { get; init; }
}
