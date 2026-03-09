namespace Catalog.Application.Features.Products.Create;

public record CreateProductInput(
    string Name,
    string Description,
    Guid BrandId,
    Guid CategoryId,
    decimal PriceAmount,
    string PriceCurrency = "BRL",
    string Sku = "",
    string? Barcode = null,
    int InitialStock = 0,
    int? WeightInGrams = null,
    decimal? LengthInCm = null,
    decimal? WidthInCm = null,
    decimal? HeightInCm = null,
    string? SeoTitle = null,
    string? SeoDescription = null);
