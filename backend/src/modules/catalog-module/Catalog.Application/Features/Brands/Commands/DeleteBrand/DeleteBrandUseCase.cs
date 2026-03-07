using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Shared.Application.Models;
using FluentValidation;

namespace Catalog.Application.Features.Brands.Commands.DeleteBrand;

public class DeleteBrandUseCase : IDeleteBrandUseCase
{
    private readonly IBrandRepository _brandRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteBrandInput> _validator;

    public DeleteBrandUseCase(
        IBrandRepository brandRepository,
        ICatalogUnitOfWork unitOfWork,
        IValidator<DeleteBrandInput> validator)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Result>> ExecuteAsync(DeleteBrandInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validation
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Result>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Fetch Entity
        var brand = await _brandRepository.GetByIdAsync(input.Id, cancellationToken);
        if (brand is null || brand.IsDeleted)
        {
            return Result.Failure<Result>(BrandErrors.NotFound(input.Id));
        }

        // 3. Rules constraints checkout (e.g. Products relationship)
        var hasProducts = await _brandRepository.HasProductsAsync(input.Id, cancellationToken);
        if (hasProducts)
        {
            return Result.Failure<Result>(BrandErrors.CannotDeleteWithProducts);
        }

        // 4. Soft Delete Execution (and event raise mapped in Domain Entity)
        brand.Delete();

        // 5. Persistence
        _brandRepository.Update(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Return wrapper
        return Result.Success(Result.Success());
    }
}
