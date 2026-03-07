
using Shared.Domain.Exceptions;

namespace Catalog.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um usuário tenta avaliar um produto que já avaliou.
/// </summary>
public class DuplicateReviewException : DomainException
{
    public Guid ProductId { get; }
    public Guid UserId { get; }

    public DuplicateReviewException(Guid productId, Guid userId)
        : base("DUPLICATE_REVIEW", 
            $"O usuário {userId} já avaliou o produto {productId}. Cada usuário pode avaliar um produto apenas uma vez.")
    {
        ProductId = productId;
        UserId = userId;
    }
}
