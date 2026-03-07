using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento de domínio disparado quando um produto é publicado.
/// </summary>
/// <param name="ProductId">ID do produto publicado.</param>
/// <param name="Sku">SKU do produto.</param>
/// <param name="Name">Nome do produto.</param>
/// <param name="PublishedAt">Data/hora da publicação.</param>
public sealed class ProductPublishedDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public string Sku { get; }
    public string Name { get; }
    public DateTime PublishedAt { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductPublishedDomainEvent(Guid productId, string sku, string name, DateTime publishedAt)
    {
        ProductId = productId;
        Sku = sku;
        Name = name;
        PublishedAt = publishedAt;
    }
}



