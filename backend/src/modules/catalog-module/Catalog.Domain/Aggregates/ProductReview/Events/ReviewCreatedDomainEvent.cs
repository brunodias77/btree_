using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.ProductReview.Events;

/// <summary>
/// Evento de domínio disparado quando uma nova avaliação é criada.
/// </summary>
/// <param name="ReviewId">ID da avaliação.</param>
/// <param name="ProductId">ID do produto.</param>
/// <param name="UserId">ID do usuário.</param>
/// <param name="Rating">Nota da avaliação (1-5).</param>
/// <param name="IsVerifiedPurchase">Se é uma compra verificada.</param>
public sealed class ReviewCreatedDomainEvent : DomainEventBase
{
    public Guid ReviewId { get; }
    public Guid ProductId { get; }
    public Guid UserId { get; }
    public int Rating { get; }
    public bool IsVerifiedPurchase { get; }

    public override string AggregateType => "ProductReview";
    public override Guid AggregateId => ReviewId;
    public override string Module => "catalog";

    public ReviewCreatedDomainEvent(Guid reviewId, Guid productId, Guid userId, int rating, bool isVerifiedPurchase)
    {
        ReviewId = reviewId;
        ProductId = productId;
        UserId = userId;
        Rating = rating;
        IsVerifiedPurchase = isVerifiedPurchase;
    }
}



