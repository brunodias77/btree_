using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects;
using Shared.Application.Models;
using FluentValidation;

namespace Catalog.Application.Features.Products.Update;

public class UpdateProductUseCase : IUpdateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly ISlugGenerator _slugGenerator;
    private readonly IValidator<UpdateProductInput> _validator;

    public UpdateProductUseCase(
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork,
        ISlugGenerator slugGenerator,
        IValidator<UpdateProductInput> validator)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _slugGenerator = slugGenerator;
        _validator = validator;
    }

    public async Task<Result<Guid>> ExecuteAsync(UpdateProductInput input, CancellationToken cancellationToken = default)
    {
        // 1. Base validation
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Retrieval
        var product = await _productRepository.GetByIdAsync(input.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure<Guid>(ProductErrors.NotFound(input.Id));
        }

        // 3. Conditional Unique Validation (SKU)
        var newSku = input.Sku.Trim().ToUpperInvariant();
        if (product.Sku != newSku)
        {
            var skuExists = await _productRepository.ExistsBySkuAsync(newSku, cancellationToken);
            if (skuExists)
            {
                return Result.Failure<Guid>(ProductErrors.SkuAlreadyExists(input.Sku));
            }
        }

        // 4. Conditional Validation of Relationships
        if (product.BrandId != input.BrandId)
        {
            var brandExists = await _brandRepository.GetByIdAsync(input.BrandId, cancellationToken);
            if (brandExists is null)
            {
                return Result.Failure<Guid>(BrandErrors.NotFound(input.BrandId));
            }
        }

        if (product.CategoryId != input.CategoryId)
        {
            var categoryExists = await _categoryRepository.GetByIdAsync(input.CategoryId, cancellationToken);
            if (categoryExists is null)
            {
                return Result.Failure<Guid>(CategoryErrors.NotFound(input.CategoryId));
            }
        }

        try
        {
            // 5. Value Objects Initialization
            var money = Money.Create(input.PriceAmount, input.PriceCurrency);
            var sku = Sku.Create(input.Sku);

            Barcode? barcode = null;
            if (!string.IsNullOrWhiteSpace(input.Barcode))
            {
                barcode = Barcode.Create(input.Barcode);
            }

            Dimensions? dimensions = null;
            if (input.WeightInGrams.HasValue)
            {
                dimensions = Dimensions.Create(
                    input.WeightInGrams,
                    input.HeightInCm,
                    input.WidthInCm,
                    input.LengthInCm);
            }

            SeoMetadata? seoMetadata = null;
            if (!string.IsNullOrWhiteSpace(input.SeoTitle) || !string.IsNullOrWhiteSpace(input.SeoDescription))
            {
                seoMetadata = SeoMetadata.Create(input.SeoTitle, input.SeoDescription);
            }

            // 6. Slug generation
            var currentSlug = product.Slug;
            var targetSlug = currentSlug;

            if (!string.Equals(product.Name, input.Name, StringComparison.OrdinalIgnoreCase))
            {
                targetSlug = _slugGenerator.GenerateSlug(input.Name);
                
                var slugExists = await _productRepository.ExistsBySlugAsync(targetSlug, cancellationToken);
                if (slugExists)
                {
                    return Result.Failure<Guid>(ProductErrors.SlugAlreadyExists(targetSlug));
                }
            }

            // 7. Entity Updates
            product.Update(
                name: input.Name,
                slug: targetSlug,
                description: input.Description,
                barcode: barcode?.Value,
                metaTitle: seoMetadata?.Title,
                metaDescription: seoMetadata?.Description);

            product.UpdateSku(sku.Value);
            product.UpdatePrice(money.Amount);
            product.SetBrand(input.BrandId);
            product.SetCategory(input.CategoryId);

            if (dimensions is not null)
            {
                product.UpdateDimensions(
                    dimensions.WeightGrams,
                    dimensions.HeightCm,
                    dimensions.WidthCm,
                    dimensions.LengthCm);
            }

            // 8. Persistence
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 9. Return
            return Result.Success(product.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(Error.Validation("Domain.ValidationError", ex.Message));
        }
    }
}
