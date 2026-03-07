using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento de domínio disparado quando um novo produto é criado.
/// </summary>
/// <param name="ProductId">ID do produto criado.</param>
/// <param name="Sku">SKU do produto.</param>
/// <param name="Name">Nome do produto.</param>
/// <param name="CategoryId">ID da categoria (opcional).</param>
/// <param name="BrandId">ID da marca (opcional).</param>
public sealed class ProductCreatedDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public string Sku { get; }
    public string Name { get; }
    public Guid? CategoryId { get; }
    public Guid? BrandId { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductCreatedDomainEvent(Guid productId, string sku, string name, Guid? categoryId, Guid? brandId)
    {
        ProductId = productId;
        Sku = sku;
        Name = name;
        CategoryId = categoryId;
        BrandId = brandId;
    }
}



