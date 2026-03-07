using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Product.Events;

/// <summary>
/// Evento de domínio disparado quando uma imagem é adicionada a um produto.
/// </summary>
/// <param name="ProductId">ID do produto.</param>
/// <param name="ImageId">ID da imagem adicionada.</param>
/// <param name="ImageUrl">URL da imagem.</param>
/// <param name="IsPrimary">Se é a imagem principal.</param>
public sealed class ProductImageAddedDomainEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public Guid ImageId { get; }
    public string ImageUrl { get; }
    public bool IsPrimary { get; }

    public override string AggregateType => nameof(Product);
    public override Guid AggregateId => ProductId;
    public override string Module => "catalog";

    public ProductImageAddedDomainEvent(Guid productId, Guid imageId, string imageUrl, bool isPrimary)
    {
        ProductId = productId;
        ImageId = imageId;
        ImageUrl = imageUrl;
        IsPrimary = isPrimary;
    }
}



