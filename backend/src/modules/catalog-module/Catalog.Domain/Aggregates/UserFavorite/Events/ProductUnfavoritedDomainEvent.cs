using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.UserFavorite.Events;

/// <summary>
/// Evento de domínio disparado quando um produto é removido dos favoritos.
/// </summary>
/// <param name="FavoriteId">ID do favorito.</param>
/// <param name="UserId">ID do usuário.</param>
/// <param name="ProductId">ID do produto removido dos favoritos.</param>
public sealed class ProductUnfavoritedDomainEvent : DomainEventBase
{
    public Guid FavoriteId { get; }
    public Guid UserId { get; }
    public Guid ProductId { get; }

    public override string AggregateType => "UserFavorite";
    public override Guid AggregateId => FavoriteId;
    public override string Module => "catalog";

    public ProductUnfavoritedDomainEvent(Guid favoriteId, Guid userId, Guid productId)
    {
        FavoriteId = favoriteId;
        UserId = userId;
        ProductId = productId;
    }
}



