using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using FluentValidation;
using Shared.Application.Abstractions;
using Shared.Application.Models;

namespace Catalog.Application.Features.Review.Approve;

public class ApproveReviewUseCase : IApproveReviewUseCase
{
    private readonly IProductReviewRepository _reviewRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IValidator<ApproveReviewInput> _validator;

    public ApproveReviewUseCase(
        IProductReviewRepository reviewRepository,
        ICatalogUnitOfWork unitOfWork,
        IValidator<ApproveReviewInput> validator)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<bool>> ExecuteAsync(ApproveReviewInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validacao Input
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<bool>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Obter Review
        var review = await _reviewRepository.GetByIdAsync(input.ReviewId, cancellationToken);
        if (review is null)
        {
            return Result.Failure<bool>(ReviewErrors.NotFound(input.ReviewId));
        }

        // 3. Aprovar (validação de domínio try-catch para state)
        try
        {
            review.Approve();
        }
        catch (InvalidOperationException)
        {
            return Result.Failure<bool>(ReviewErrors.NotInPendingState);
        }

        // 4. Persistir
        _reviewRepository.Update(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Retorno
        return Result.Success(true);
    }
}
