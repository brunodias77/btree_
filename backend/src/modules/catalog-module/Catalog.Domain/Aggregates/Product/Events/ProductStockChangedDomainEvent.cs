using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento de domínio disparado quando o estoque de um produto é alterado.
/// </summary>
/// <param name="ProductId">ID do produto.</param>
/// <param name="Sku">SKU do produto.</param>
/// <param name="OldStock">Estoque anterior.</param>
/// <param name="NewStock">Novo estoque.</param>
/// <param name="ChangeReason">Motivo da alteração.</param>
public sealed class ProductStockChangedDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public string Sku { get; }
    public int OldStock { get; }
    public int NewStock { get; }
    public string? ChangeReason { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductStockChangedDomainEvent(Guid productId, string sku, int oldStock, int newStock, string? changeReason)
    {
        ProductId = productId;
        Sku = sku;
        OldStock = oldStock;
        NewStock = newStock;
        ChangeReason = changeReason;
    }
}



