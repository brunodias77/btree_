using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.ProductReview.Events;

/// <summary>
/// Evento de domínio disparado quando uma avaliação de produto é deletada (soft delete).
/// </summary>
public sealed class ReviewDeletedDomainEvent : DomainEventBase
{
    public Guid ReviewId { get; }
    public Guid ProductId { get; }

    public override string AggregateType => nameof(ProductReview);
    public override Guid AggregateId => ReviewId;
    public override string Module => "catalog";

    public ReviewDeletedDomainEvent(Guid reviewId, Guid productId)
    {
        ReviewId = reviewId;
        ProductId = productId;
    }
}
