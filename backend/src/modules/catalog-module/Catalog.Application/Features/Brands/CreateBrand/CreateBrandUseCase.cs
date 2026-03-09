using Catalog.Domain.Aggregates.Brand;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Catalog.Domain.Services;
using Shared.Application.Models;
using FluentValidation;

namespace Catalog.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandUseCase : ICreateBrandUseCase
{
    private readonly IBrandRepository _brandRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly ISlugGenerator _slugGenerator;
    private readonly IValidator<CreateBrandInput> _validator;

    public CreateBrandUseCase(
        IBrandRepository brandRepository,
        ICatalogUnitOfWork unitOfWork,
        ISlugGenerator slugGenerator,
        IValidator<CreateBrandInput> validator)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _slugGenerator = slugGenerator;
        _validator = validator;
    }

    public async Task<Result<Guid>> ExecuteAsync(CreateBrandInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validation
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Uniqueness verification by Name
        var exists = await _brandRepository.ExistsByNameAsync(input.Name, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(BrandErrors.DuplicateName);
        }

        // 3. Slug generation
        var generatedSlug = _slugGenerator.GenerateSlug(input.Name);

        // 4. Checking initial Slug collision possibilities
        var slugExists = await _brandRepository.ExistsBySlugAsync(generatedSlug, cancellationToken);
        if (slugExists)
        {
            return Result.Failure<Guid>(BrandErrors.SlugAlreadyExists(generatedSlug));
        }

        // 5. Build Aggregate
        var brand = Brand.Create(
            name: input.Name,
            slug: generatedSlug,
            description: input.Description,
            logoUrl: input.LogoUrl);

        if (!input.IsActive)
        {
            brand.Deactivate();
        }

        // 6. Persistence & Events Generation
        await _brandRepository.AddAsync(brand, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Return Result
        return Result.Success(brand.Id);
    }
}
