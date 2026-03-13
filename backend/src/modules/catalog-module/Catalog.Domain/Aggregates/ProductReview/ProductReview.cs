using Catalog.Domain.Aggregates.ProductReview.Events;
using Catalog.Domain.Enums;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.Aggregates.ProductReview;

/// <summary>
/// Aggregate Root representando uma avaliação de produto.
/// </summary>
public sealed class ProductReview : AggregateRoot<Guid>
{
    /// <summary>
    /// ID do produto avaliado.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// ID do usuário que fez a avaliação.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// ID do pedido relacionado (para compras verificadas).
    /// </summary>
    public Guid? OrderId { get; private set; }

    /// <summary>
    /// Nota da avaliação (1 a 5 estrelas).
    /// </summary>
    public int Rating { get; private set; }

    /// <summary>
    /// Título da avaliação.
    /// </summary>
    public string? Title { get; private set; }

    /// <summary>
    /// Comentário detalhado da avaliação.
    /// </summary>
    public string? Comment { get; private set; }

    /// <summary>
    /// Indica se é uma compra verificada.
    /// </summary>
    public bool IsVerifiedPurchase { get; private set; }

    /// <summary>
    /// Status de aprovação da avaliação.
    /// </summary>
    public ReviewStatus Status { get; private set; } = ReviewStatus.Pending;

    /// <summary>
    /// Indica se a avaliação está aprovada e visível.
    /// </summary>
    public bool IsApproved => Status == ReviewStatus.Approved;

    /// <summary>
    /// Resposta do vendedor à avaliação.
    /// </summary>
    public string? SellerResponse { get; private set; }

    /// <summary>
    /// Data/hora da resposta do vendedor.
    /// </summary>
    public DateTime? SellerRespondedAt { get; private set; }

    /// <summary>
    /// Construtor privado para EF Core.
    /// </summary>
    private ProductReview() : base() { }

    /// <summary>
    /// Construtor privado para criação da avaliação.
    /// </summary>
    private ProductReview(
        Guid id,
        Guid productId,
        Guid userId,
        int rating) : base(id)
    {
        ProductId = productId;
        UserId = userId;
        Rating = rating;
    }

    /// <summary>
    /// Factory method para criar uma nova avaliação de produto.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="rating">Nota (1 a 5).</param>
    /// <param name="title">Título da avaliação (opcional).</param>
    /// <param name="comment">Comentário (opcional).</param>
    /// <param name="orderId">ID do pedido para compras verificadas (opcional).</param>
    /// <returns>Nova instância de ProductReview.</returns>
    public static ProductReview Create(
        Guid productId,
        Guid userId,
        int rating,
        string? title = null,
        string? comment = null,
        Guid? orderId = null)
    {
        ValidateRating(rating);

        var review = new ProductReview(Guid.NewGuid(), productId, userId, rating)
        {
            Title = title?.Trim(),
            Comment = comment?.Trim(),
            OrderId = orderId,
            IsVerifiedPurchase = orderId.HasValue
        };

        review.RegisterDomainEvent(new ReviewCreatedDomainEvent(
            review.Id,
            review.ProductId,
            review.UserId,
            review.Rating,
            review.IsVerifiedPurchase));

        return review;
    }

    /// <summary>
    /// Atualiza a avaliação.
    /// </summary>
    public void Update(int rating, string? title, string? comment)
    {
        ValidateRating(rating);

        Rating = rating;
        Title = title?.Trim();
        Comment = comment?.Trim();
        
        // Voltar para pendente se já estava aprovada
        if (Status == ReviewStatus.Approved)
        {
            Status = ReviewStatus.Pending;
        }
        
        IncrementVersion();

        RegisterDomainEvent(new ReviewUpdatedDomainEvent(
            Id,
            ProductId,
            Rating
        ));
    }

    /// <summary>
    /// Aprova a avaliação para exibição pública.
    /// </summary>
    public void Approve()
    {
        if (Status != ReviewStatus.Pending)
        {
            throw new InvalidOperationException("A avaliação não está em estado pendente.");
        }

        Status = ReviewStatus.Approved;
        IncrementVersion();

        RegisterDomainEvent(new ReviewApprovedDomainEvent(
            Id,
            ProductId,
            UserId,
            Rating));
    }

    /// <summary>
    /// Rejeita a avaliação.
    /// </summary>
    /// <param name="reason">Motivo da rejeição.</param>
    public void Reject(string? reason = null)
    {
        if (Status == ReviewStatus.Rejected)
            return;

        Status = ReviewStatus.Rejected;
        IncrementVersion();

        RegisterDomainEvent(new ReviewRejectedDomainEvent(
            Id,
            ProductId,
            UserId,
            reason));
    }

    /// <summary>
    /// Adiciona resposta do vendedor à avaliação.
    /// </summary>
    /// <param name="response">Texto da resposta.</param>
    public void AddSellerResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            throw new ArgumentException("A resposta do vendedor não pode ser vazia.", nameof(response));

        SellerResponse = response.Trim();
        SellerRespondedAt = DateTime.UtcNow;
        IncrementVersion();

        RegisterDomainEvent(new SellerRespondedDomainEvent(
            Id,
            ProductId,
            UserId,
            SellerResponse));
    }

    /// <summary>
    /// Remove a resposta do vendedor.
    /// </summary>
    public void RemoveSellerResponse()
    {
        SellerResponse = null;
        SellerRespondedAt = null;
        IncrementVersion();
    }

    /// <summary>
    /// Marca a avaliação como excluída (soft delete).
    /// </summary>
    public override void Delete()
    {
        if (IsDeleted) return;

        base.Delete();
        IncrementVersion();

        RegisterDomainEvent(new ReviewDeletedDomainEvent(
            Id,
            ProductId
        ));
    }

    /// <summary>
    /// Valida a nota da avaliação.
    /// </summary>
    private static void ValidateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("A nota deve estar entre 1 e 5.", nameof(rating));
    }
}
