using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento disparado quando uma imagem de um produto é removida.
/// </summary>
public sealed class ProductImageRemovedDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public Guid ImageId { get; }
    public string ImageUrl { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductImageRemovedDomainEvent(Guid productId, Guid imageId, string imageUrl)
    {
        ProductId = productId;
        ImageId = imageId;
        ImageUrl = imageUrl;
    }
}
