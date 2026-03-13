using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using FluentValidation;
using Shared.Application.Abstractions;
using Shared.Application.Models;

namespace Catalog.Application.Features.Review.Delete;

public class DeleteReviewUseCase : IDeleteReviewUseCase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProductReviewRepository _reviewRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteReviewInput> _validator;

    public DeleteReviewUseCase(
        ICurrentUserService currentUserService,
        IProductReviewRepository reviewRepository,
        ICatalogUnitOfWork unitOfWork,
        IValidator<DeleteReviewInput> validator)
    {
        _currentUserService = currentUserService;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<bool>> ExecuteAsync(DeleteReviewInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validacao Input Base
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<bool>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Utilizador Autenticado e Permissões
        var userId = _currentUserService.UserId;
        var isAdmin = _currentUserService.Roles?.Contains("Admin") ?? false;

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

        // 4. Verificacao de Autoria (Ownership) ou Admin Role
        if (review.UserId != userId.Value && !isAdmin)
        {
            return Result.Failure<bool>(ReviewErrors.NotOwner);
        }

        // 5. Excluir Review (Soft Delete + Evento)
        review.Delete();

        // 6. Persistencia
        _reviewRepository.Update(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retorno
        return Result.Success(true);
    }
}
