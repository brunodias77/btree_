using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Catalog.Domain.Services;
using Shared.Application.Models;
using FluentValidation;

namespace Catalog.Application.Features.Brands.UpdateBrand;

public class UpdateBrandUseCase : IUpdateBrandUseCase
{
    private readonly IBrandRepository _brandRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly ISlugGenerator _slugGenerator;
    private readonly IValidator<UpdateBrandInput> _validator;

    public UpdateBrandUseCase(
        IBrandRepository brandRepository,
        ICatalogUnitOfWork unitOfWork,
        ISlugGenerator slugGenerator,
        IValidator<UpdateBrandInput> validator)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _slugGenerator = slugGenerator;
        _validator = validator;
    }

    public async Task<Result<Guid>> ExecuteAsync(UpdateBrandInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validation
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Fetch Entity
        var brand = await _brandRepository.GetByIdAsync(input.Id, cancellationToken);
        if (brand is null || brand.IsDeleted)
        {
            return Result.Failure<Guid>(BrandErrors.NotFound(input.Id));
        }

        var slug = brand.Slug;

        // 3. Name update rules and uniqueness verification
        if (!string.Equals(input.Name.Trim(), brand.Name.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _brandRepository.ExistsByNameAsync(input.Name, cancellationToken);
            if (exists)
            {
                return Result.Failure<Guid>(BrandErrors.DuplicateName);
            }

            slug = _slugGenerator.GenerateSlug(input.Name);

            var slugExists = await _brandRepository.ExistsBySlugExcludingAsync(slug, brand.Id, cancellationToken);
            if (slugExists)
            {
                return Result.Failure<Guid>(BrandErrors.SlugAlreadyExists(slug));
            }
        }

        // 4. Update the Aggregate Root properties
        brand.Update(
            name: input.Name,
            slug: slug,
            description: input.Description,
            logoUrl: input.LogoUrl,
            websiteUrl: brand.WebsiteUrl, // Updating what isn't asked keeps unchanged
            sortOrder: brand.SortOrder);

        // 5. Activation constraints
        if (input.IsActive && !brand.IsActive)
        {
            brand.Activate();
        }
        else if (!input.IsActive && brand.IsActive)
        {
            brand.Deactivate();
        }

        // 6. Persistence & Events Dispatching
        _brandRepository.Update(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Result wrap
        return Result.Success(brand.Id);
    }
}
