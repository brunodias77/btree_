using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using FluentValidation;
using Shared.Application.Abstractions;
using Shared.Application.Models;

namespace Catalog.Application.Features.Review.Update;

public class UpdateReviewUseCase : IUpdateReviewUseCase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProductReviewRepository _reviewRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateReviewInput> _validator;

    public UpdateReviewUseCase(
        ICurrentUserService currentUserService,
        IProductReviewRepository reviewRepository,
        ICatalogUnitOfWork unitOfWork,
        IValidator<UpdateReviewInput> validator)
    {
        _currentUserService = currentUserService;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<bool>> ExecuteAsync(UpdateReviewInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validacao Input Base
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<bool>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Utilizador Autenticado
        var userId = _currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            return Result.Failure<bool>(Error.Unauthorized("Review.Unauthorized", "Acesso não autorizado ou utilizador não identificado."));
        }

        // 3. Obter Review
        var review = await _reviewRepository.GetByIdAsync(input.ReviewId, cancellationToken);
        if (review is null)
        {
            return Result.Failure<bool>(ReviewErrors.NotFound(input.ReviewId));
        }

        // 4. Verificacao de Autoria (Ownership)
        if (review.UserId != userId.Value)
        {
            return Result.Failure<bool>(ReviewErrors.NotOwner);
        }

        // 5. Atualizar Review com a regra de negocio da root aggregate
        try
        {
            review.Update(
                rating: input.Rating,
                title: input.Title,
                comment: input.Comment
            );
        }
        catch (ArgumentException)
        {
            return Result.Failure<bool>(ReviewErrors.InvalidRating);
        }

        // 6. Persistencia
        _reviewRepository.Update(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retorno
        return Result.Success(true);
    }
}
