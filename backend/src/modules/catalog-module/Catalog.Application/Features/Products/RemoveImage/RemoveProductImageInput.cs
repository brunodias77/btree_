namespace Catalog.Application.Features.Products.RemoveImage;

public record RemoveProductImageInput(
    Guid ProductId,
    Guid ImageId);
