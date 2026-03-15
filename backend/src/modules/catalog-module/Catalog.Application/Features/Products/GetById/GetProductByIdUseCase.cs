using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using FluentValidation;
using Shared.Application.Abstractions;
using Shared.Application.Models;

namespace Catalog.Application.Features.Products.GetById;

/// <summary>
/// Implementação do caso de uso de consulta detalhada de produto por ID.
/// Utiliza IProductReadRepository e ICacheService para otimização em conformidade com o CQRS.
/// </summary>
public class GetProductByIdUseCase : IGetProductByIdUseCase
{
    private readonly IProductReadRepository _readRepository;
    private readonly ICacheService _cacheService;
    private readonly IValidator<GetProductByIdInput> _validator;

    public GetProductByIdUseCase(
        IProductReadRepository readRepository,
        ICacheService cacheService,
        IValidator<GetProductByIdInput> validator)
    {
        _readRepository = readRepository;
        _cacheService = cacheService;
        _validator = validator;
    }

    public async Task<Result<ProductOutput>> ExecuteAsync(
        GetProductByIdInput input,
        CancellationToken cancellationToken = default)
    {
        // 1. Validation
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<ProductOutput>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Cache Strategy
        var cacheKey = $"catalog:products:{input.Id}";
        
        var cachedProduct = await _cacheService.GetAsync<ProductOutput>(cacheKey, cancellationToken);
        if (cachedProduct is not null)
        {
            return Result.Success(cachedProduct);
        }

        // 3. Database Query
        // O repositório IProductReadRepository.GetByIdAsync deve garantir soft-delete e os includes necessários
        var product = await _readRepository.GetByIdAsync(input.Id, cancellationToken);

        // 4. Business Rule: Not Found or Deleted
        if (product is null)
        {
            return Result.Failure<ProductOutput>(ProductErrors.NotFound(input.Id));
        }

        // 5. Mapping
        var imagesOutput = product.Images
            .Select(img => new ProductImageOutput(img.Id, img.Url, img.IsPrimary))
            .ToList();

        var output = new ProductOutput(
            Id: product.Id,
            Name: product.Name,
            Description: product.Description ?? string.Empty,
            Sku: product.Sku,
            Price: product.Price,
            QuantityInStock: product.Stock,
            Status: product.Status.ToString(),
            BrandName: product.BrandId?.ToString() ?? string.Empty, 
            CategoryName: product.CategoryId?.ToString() ?? string.Empty, 
            Images: imagesOutput
        );

        // 6. Atualização do Cache TTL 15 minutos
        await _cacheService.SetAsync(cacheKey, output, TimeSpan.FromMinutes(15), cancellationToken);

        // 7. Retorno
        return Result.Success(output);
    }
}
