namespace Catalog.Application.Features.Products.AddImage;

public record AddProductImageInput(
    Guid ProductId,
    Stream ImageStream,
    string FileName,
    string ContentType,
    bool IsPrimary = false);
