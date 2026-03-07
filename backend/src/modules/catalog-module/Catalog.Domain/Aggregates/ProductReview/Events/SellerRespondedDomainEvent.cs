using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.ProductReview.Events;

/// <summary>
/// Evento de domínio disparado quando o vendedor responde a uma avaliação.
/// </summary>
/// <param name="ReviewId">ID da avaliação.</param>
/// <param name="ProductId">ID do produto.</param>
/// <param name="UserId">ID do usuário que fez a avaliação.</param>
/// <param name="Response">Resposta do vendedor.</param>
public sealed class SellerRespondedDomainEvent : DomainEventBase
{
    public Guid ReviewId { get; }
    public Guid ProductId { get; }
    public Guid UserId { get; }
    public string Response { get; }

    public override string AggregateType => "ProductReview";
    public override Guid AggregateId => ReviewId;
    public override string Module => "catalog";

    public SellerRespondedDomainEvent(Guid reviewId, Guid productId, Guid userId, string response)
    {
        ReviewId = reviewId;
        ProductId = productId;
        UserId = userId;
        Response = response;
    }
}



