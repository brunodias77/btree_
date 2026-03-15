using Catalog.Domain.Repositories;
using Shared.Application.Models;
using FluentValidation;

namespace Catalog.Application.Features.Products.GetAllProducts;

public class GetProductsUseCase : IGetProductsUseCase
{
    private readonly IProductReadRepository _readRepository;
    private readonly IValidator<GetProductsInput> _validator;

    // UnitOfWork and write Repositories are intentionally omitted since this is a Query Use Case.
    public GetProductsUseCase(
        IProductReadRepository readRepository,
        IValidator<GetProductsInput> validator)
    {
        _readRepository = readRepository;
        _validator = validator;
    }

    public async Task<Result<PagedResult<ProductOutput>>> ExecuteAsync(GetProductsInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validate Input (Paging rules, whitelist SortBy)
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PagedResult<ProductOutput>>(new Error("Validation.Error", validationResult.ToString()));
        }
        
        // 2. Normalize Pagination Inputs (Security & Boundaries from the base PagedRequest)
        var normalizedInput = (GetProductsInput)input.Normalize();

        // 3. Delegate to Data Access (ReadRepository)
        // Notice we don't return Result.Failure if the list is empty. We merely return an Empty array success.
        var pagedEntities = await _readRepository.GetByFilterPagedAsync(
            page: normalizedInput.PageNumber,
            pageSize: normalizedInput.PageSize,
            searchTerm: normalizedInput.SearchTerm,
            categoryId: normalizedInput.CategoryId,
            brandId: normalizedInput.BrandId,
            status: normalizedInput.Status,
            orderBy: normalizedInput.SortBy,
            orderDirection: normalizedInput.SortDirection,
            cancellationToken: cancellationToken);

        if (pagedEntities.TotalCount == 0)
        {
            return Result.Success(PagedResult<ProductOutput>.Empty(normalizedInput.PageSize));
        }

        // 4. Projection
        // We use Map to transform the Product aggregates coming from EF into DTOs
        var pagedOutput = pagedEntities.Map(product => new ProductOutput(
            Id: product.Id,
            Name: product.Name,
            Slug: product.Slug,
            Sku: product.Sku,
            Price: product.Price,
            Currency: "BRL", // Based on previous default pattern mapping
            StockQuantity: product.Stock,
            BrandName: product.BrandId?.ToString() ?? string.Empty,
            CategoryName: product.CategoryId?.ToString() ?? string.Empty, 
            Status: product.Status.ToString(),
            MainImageUrl: product.Images.FirstOrDefault(img => img.IsPrimary)?.Url ?? product.Images.FirstOrDefault()?.Url
        ));

        // 5. Return success Wrapper
        return Result.Success(pagedOutput);
    }
}
