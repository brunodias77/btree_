using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento de domínio disparado quando um produto fica sem estoque.
/// </summary>
/// <param name="ProductId">ID do produto.</param>
/// <param name="Sku">SKU do produto.</param>
/// <param name="Name">Nome do produto.</param>
public sealed class ProductOutOfStockDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public string Sku { get; }
    public string Name { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductOutOfStockDomainEvent(Guid productId, string sku, string name)
    {
        ProductId = productId;
        Sku = sku;
        Name = name;
    }
}



