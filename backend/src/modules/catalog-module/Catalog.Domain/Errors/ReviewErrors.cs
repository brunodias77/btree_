
using Shared.Application.Models;

namespace Catalog.Domain.Errors;

/// <summary>
/// Erros relacionados a avaliações de produtos.
/// </summary>
public static class ReviewErrors
{
    public static Error NotFound(Guid reviewId) =>
        Error.NotFound("ProductReview", reviewId);

    public static Error AlreadyReviewed(Guid productId, Guid userId) =>
        Error.Conflict("ProductReview", 
            $"O usuário {userId} já avaliou o produto {productId}. Cada usuário pode avaliar um produto apenas uma vez.");

    public static Error InvalidRating =>
        Error.Validation("Review.Rating", "A nota deve estar entre 1 e 5.");

    public static Error RatingRequired =>
        Error.Validation("Review.Rating", "A nota é obrigatória.");

    public static Error NotVerifiedPurchase =>
        Error.Failure("Review.NotVerifiedPurchase", 
            "Você precisa comprar este produto para avaliá-lo como compra verificada.");

    public static Error AlreadyApproved =>
        Error.Failure("Review.AlreadyApproved", "Esta avaliação já foi aprovada.");

    public static Error AlreadyRejected =>
        Error.Failure("Review.AlreadyRejected", "Esta avaliação já foi rejeitada.");

    public static Error CannotModifyApproved =>
        Error.Failure("Review.CannotModifyApproved", 
            "Não é possível modificar uma avaliação já aprovada.");

    public static Error SellerResponseRequired =>
        Error.Validation("Review.SellerResponse", "A resposta do vendedor não pode ser vazia.");

    public static Error SellerAlreadyResponded =>
        Error.Failure("Review.SellerAlreadyResponded", "O vendedor já respondeu a esta avaliação.");

    public static Error NotOwner =>
        Error.Forbidden("Review.NotOwner", "Você não tem permissão para modificar esta avaliação porque não é o proprietário.");

    public static Error NotInPendingState =>
        Error.Validation("Review.NotInPendingState", "A avaliação não se encontra em estado pendente para sofrer esta transição.");
}
