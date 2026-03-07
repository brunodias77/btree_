using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento de domínio disparado quando um produto é excluído (soft delete).
/// </summary>
/// <param name="ProductId">ID do produto excluído.</param>
/// <param name="Sku">SKU do produto excluído.</param>
/// <param name="Name">Nome do produto excluído.</param>
public sealed class ProductDeletedDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public string Sku { get; }
    public string Name { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductDeletedDomainEvent(Guid productId, string sku, string name)
    {
        ProductId = productId;
        Sku = sku;
        Name = name;
    }
}



