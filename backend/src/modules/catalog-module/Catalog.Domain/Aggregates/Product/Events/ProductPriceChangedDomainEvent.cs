using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento de domínio disparado quando o preço de um produto é alterado.
/// </summary>
/// <param name="ProductId">ID do produto.</param>
/// <param name="Sku">SKU do produto.</param>
/// <param name="OldPrice">Preço anterior.</param>
/// <param name="NewPrice">Novo preço.</param>
/// <param name="OldCompareAtPrice">Preço de comparação anterior.</param>
/// <param name="NewCompareAtPrice">Novo preço de comparação.</param>
public sealed class ProductPriceChangedDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public string Sku { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }
    public decimal? OldCompareAtPrice { get; }
    public decimal? NewCompareAtPrice { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductPriceChangedDomainEvent(Guid productId, string sku, decimal oldPrice, decimal newPrice, decimal? oldCompareAtPrice, decimal? newCompareAtPrice)
    {
        ProductId = productId;
        Sku = sku;
        OldPrice = oldPrice;
        NewPrice = newPrice;
        OldCompareAtPrice = oldCompareAtPrice;
        NewCompareAtPrice = newCompareAtPrice;
    }
}



