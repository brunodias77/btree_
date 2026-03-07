using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.ProductReview.Events;

/// <summary>
/// Evento de domínio disparado quando uma avaliação é aprovada.
/// </summary>
/// <param name="ReviewId">ID da avaliação.</param>
/// <param name="ProductId">ID do produto.</param>
/// <param name="UserId">ID do usuário.</param>
/// <param name="Rating">Nota da avaliação (1-5).</param>
public sealed class ReviewApprovedDomainEvent : DomainEventBase
{
    public Guid ReviewId { get; }
    public Guid ProductId { get; }
    public Guid UserId { get; }
    public int Rating { get; }

    public override string AggregateType => "ProductReview";
    public override Guid AggregateId => ReviewId;
    public override string Module => "catalog";

    public ReviewApprovedDomainEvent(Guid reviewId, Guid productId, Guid userId, int rating)
    {
        ReviewId = reviewId;
        ProductId = productId;
        UserId = userId;
        Rating = rating;
    }
}



