namespace Catalog.Application.Features.Products.GetAllProducts;

public record ProductOutput(
    Guid Id,
    string Name,
    string Slug,
    string Sku,
    decimal Price,
    string Currency,
    int StockQuantity,
    string BrandName,
    string CategoryName,
    string Status,
    string? MainImageUrl);
