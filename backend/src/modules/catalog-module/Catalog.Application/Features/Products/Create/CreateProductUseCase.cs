using Catalog.Domain.Aggregates.Product;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects;
using Shared.Application.Models;
using FluentValidation;

namespace Catalog.Application.Features.Products.Create;

public class CreateProductUseCase : ICreateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly ISlugGenerator _slugGenerator;
    private readonly IValidator<CreateProductInput> _validator;

    public CreateProductUseCase(
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork,
        ISlugGenerator slugGenerator,
        IValidator<CreateProductInput> validator)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _slugGenerator = slugGenerator;
        _validator = validator;
    }

    public async Task<Result<Guid>> ExecuteAsync(CreateProductInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validacao de Campos Base
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Uniqueness verification by SKU
        var skuExists = await _productRepository.ExistsBySkuAsync(input.Sku, cancellationToken);
        if (skuExists)
        {
            return Result.Failure<Guid>(ProductErrors.SkuAlreadyExists(input.Sku));
        }

        // 3. Validation of Relationships (Associations)
        var brandExists = await _brandRepository.GetByIdAsync(input.BrandId, cancellationToken);
        if (brandExists is null)
        {
            return Result.Failure<Guid>(BrandErrors.NotFound(input.BrandId));
        }

        var categoryExists = await _categoryRepository.GetByIdAsync(input.CategoryId, cancellationToken);
        if (categoryExists is null)
        {
            return Result.Failure<Guid>(CategoryErrors.NotFound(input.CategoryId));
        }

        try
        {
            // 4. Value Objects Initialization
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

            // 5. Slug generation
            var generatedSlug = _slugGenerator.GenerateSlug(input.Name);
            
            var slugExists = await _productRepository.ExistsBySlugAsync(generatedSlug, cancellationToken);
            if (slugExists)
            {
                return Result.Failure<Guid>(ProductErrors.SlugAlreadyExists(generatedSlug));
            }

            // 6. Aggregate Creation
            var product = Product.Create(
                sku: sku.Value,
                slug: generatedSlug,
                name: input.Name,
                price: money.Amount,
                categoryId: input.CategoryId,
                brandId: input.BrandId);

            product.Update(
                name: input.Name,
                slug: generatedSlug,
                description: input.Description,
                barcode: barcode?.Value,
                metaTitle: seoMetadata?.Title,
                metaDescription: seoMetadata?.Description);

            if (dimensions is not null)
            {
                product.UpdateDimensions(
                    dimensions.WeightGrams,
                    dimensions.HeightCm,
                    dimensions.WidthCm,
                    dimensions.LengthCm);
            }

            if (input.InitialStock > 0)
            {
                product.UpdateStock(input.InitialStock, "Estoque inicial configurado na criação");
            }

            // 7. Persistence
            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 8. Return
            return Result.Success(product.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(Error.Validation("Domain.ValidationError", ex.Message));
        }
    }
}
