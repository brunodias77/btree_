using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.ProductReview.Events;

/// <summary>
/// Evento de domínio disparado quando uma avaliação de produto é atualizada.
/// </summary>
public sealed class ReviewUpdatedDomainEvent : DomainEventBase
{
    public Guid ReviewId { get; }
    public Guid ProductId { get; }
    public int NewRating { get; }

    public override string AggregateType => nameof(ProductReview);
    public override Guid AggregateId => ReviewId;
    public override string Module => "catalog";

    public ReviewUpdatedDomainEvent(Guid reviewId, Guid productId, int newRating)
    {
        ReviewId = reviewId;
        ProductId = productId;
        NewRating = newRating;
    }
}
