namespace Catalog.Application.Features.Products.Update;

public record UpdateProductInput(
    Guid Id,
    string Name,
    string Description,
    Guid BrandId,
    Guid CategoryId,
    decimal PriceAmount,
    string PriceCurrency,
    string Sku,
    string? Barcode = null,
    int? WeightInGrams = null,
    decimal? LengthInCm = null,
    decimal? WidthInCm = null,
    decimal? HeightInCm = null,
    string? SeoTitle = null,
    string? SeoDescription = null);
