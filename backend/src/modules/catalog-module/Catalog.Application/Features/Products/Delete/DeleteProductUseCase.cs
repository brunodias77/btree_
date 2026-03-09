using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Shared.Application.Models;
using FluentValidation;

namespace Catalog.Application.Features.Products.Delete;

public class DeleteProductUseCase : IDeleteProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteProductInput> _validator;

    public DeleteProductUseCase(
        IProductRepository productRepository,
        ICatalogUnitOfWork unitOfWork,
        IValidator<DeleteProductInput> validator)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Result>> ExecuteAsync(DeleteProductInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validation
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Result>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Entity Retrieval
        var product = await _productRepository.GetByIdAsync(input.Id, cancellationToken);

        // 3. Verifying Existence and Idempotency 
        if (product is null)
        {
            return Result.Failure<Result>(ProductErrors.NotFound(input.Id));
        }

        if (product.IsDeleted)
        {
            return Result.Success(Result.Success());
        }

        // 4. Validate against active reserved stock
        if (product.ReservedStock > 0)
        {
            return Result.Failure<Result>(ProductErrors.CannotDeleteWithActiveReservations);
        }

        // 5. Execution of Domain Logic (Soft Delete & Events)
        product.Delete();

        // 6. Persistence
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Return
        return Result.Success(Result.Success());
    }
}
