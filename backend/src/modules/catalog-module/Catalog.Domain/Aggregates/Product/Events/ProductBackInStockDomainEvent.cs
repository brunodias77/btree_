using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento de domínio disparado quando um produto volta a ter estoque disponível.
/// </summary>
/// <param name="ProductId">ID do produto.</param>
/// <param name="Sku">SKU do produto.</param>
/// <param name="Name">Nome do produto.</param>
/// <param name="CurrentStock">Estoque atual.</param>
public sealed class ProductBackInStockDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public string Sku { get; }
    public string Name { get; }
    public int CurrentStock { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductBackInStockDomainEvent(Guid productId, string sku, string name, int currentStock)
    {
        ProductId = productId;
        Sku = sku;
        Name = name;
        CurrentStock = currentStock;
    }
}



