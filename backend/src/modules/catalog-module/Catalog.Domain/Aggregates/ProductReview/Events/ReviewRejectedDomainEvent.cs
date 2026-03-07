using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.ProductReview.Events;

/// <summary>
/// Evento de domínio disparado quando uma avaliação é rejeitada.
/// </summary>
/// <param name="ReviewId">ID da avaliação.</param>
/// <param name="ProductId">ID do produto.</param>
/// <param name="UserId">ID do usuário.</param>
/// <param name="Reason">Motivo da rejeição.</param>
public sealed class ReviewRejectedDomainEvent : DomainEventBase
{
    public Guid ReviewId { get; }
    public Guid ProductId { get; }
    public Guid UserId { get; }
    public string? Reason { get; }

    public override string AggregateType => "ProductReview";
    public override Guid AggregateId => ReviewId;
    public override string Module => "catalog";

    public ReviewRejectedDomainEvent(Guid reviewId, Guid productId, Guid userId, string? reason)
    {
        ReviewId = reviewId;
        ProductId = productId;
        UserId = userId;
        Reason = reason;
    }
}



