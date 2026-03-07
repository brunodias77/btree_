using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento de domínio disparado quando um produto é atualizado.
/// </summary>
/// <param name="ProductId">ID do produto atualizado.</param>
/// <param name="Name">Novo nome do produto.</param>
/// <param name="Slug">Novo slug do produto.</param>
public sealed class ProductUpdatedDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public string Name { get; }
    public string Slug { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductUpdatedDomainEvent(Guid productId, string name, string slug)
    {
        ProductId = productId;
        Name = name;
        Slug = slug;
    }
}



