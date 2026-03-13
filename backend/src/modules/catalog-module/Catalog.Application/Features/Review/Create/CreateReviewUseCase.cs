using Catalog.Domain.Aggregates.ProductReview;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using FluentValidation;
using Shared.Application.Abstractions;
using Shared.Application.Models;

namespace Catalog.Application.Features.Review.Create;

public class CreateReviewUseCase : ICreateReviewUseCase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProductRepository _productRepository;
    private readonly IProductReviewRepository _reviewRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IValidator<CreateReviewInput> _validator;

    public CreateReviewUseCase(
        ICurrentUserService currentUserService,
        IProductRepository productRepository,
        IProductReviewRepository reviewRepository,
        ICatalogUnitOfWork unitOfWork,
        IValidator<CreateReviewInput> validator)
    {
        _currentUserService = currentUserService;
        _productRepository = productRepository;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> ExecuteAsync(CreateReviewInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validacao Input Base
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Utilizador Autenticado
        var userId = _currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            return Result.Failure<Guid>(Error.Unauthorized("Review.Unauthorized", "Acesso não autorizado ou utilizador não identificado."));
        }

        // 3. Verificacao de Existencia do Produto
        var product = await _productRepository.GetByIdAsync(input.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure<Guid>(ProductErrors.NotFound(input.ProductId));
        }

        // 4. Verificacao de Duplicidade
        var alreadyReviewed = await _reviewRepository.ExistsByUserAndProductAsync(userId.Value, input.ProductId, cancellationToken);
        if (alreadyReviewed)
        {
            return Result.Failure<Guid>(ReviewErrors.AlreadyReviewed(input.ProductId, userId.Value));
        }

        // 5. Criacao da Review Entidade (ProductReview)
        ProductReview review;
        try
        {
            review = ProductReview.Create(
                productId: input.ProductId,
                userId: userId.Value,
                rating: input.Rating,
                title: input.Title,
                comment: input.Comment,
                orderId: null // TODO: lógica futura de verificar compra
            );
        }
        catch (ArgumentException)
        {
            return Result.Failure<Guid>(ReviewErrors.InvalidRating);
        }

        // 6. Persistencia
        await _reviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retorno
        return Result.Success(review.Id);
    }
}
