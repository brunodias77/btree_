using Catalog.Domain.Repositories;
using Shared.Application.Models;

namespace Catalog.Application.Features.Brands.GetBrands;

public class GetBrandsUseCase : IGetBrandsUseCase
{
    private readonly IBrandRepository _brandRepository;
    private readonly FluentValidation.IValidator<GetBrandsInput> _validator;

    public GetBrandsUseCase(
        IBrandRepository brandRepository,
        FluentValidation.IValidator<GetBrandsInput> validator)
    {
        _brandRepository = brandRepository;
        _validator = validator;
    }

    public async Task<Result<PagedResult<BrandOutput>>> ExecuteAsync(GetBrandsInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validação do DTO de entrada
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PagedResult<BrandOutput>>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Consulta de Leitura otimizada no Repositório
        var (items, totalCount) = await _brandRepository.BrowseAsync(
            searchTerm: input.SearchTerm,
            isActive: input.IsActive,
            page: input.PageNumber,
            pageSize: input.PageSize,
            cancellationToken: cancellationToken);

        // 3. Early return em caso de array vazio
        if (totalCount == 0)
        {
            return Result.Success(PagedResult<BrandOutput>.Empty(input.PageSize));
        }

        // 4. Mapeamento
        var outputList = items.Select(b => new BrandOutput(
            Id: b.Id,
            Name: b.Name,
            Slug: b.Slug,
            Description: b.Description,
            LogoUrl: b.LogoUrl,
            IsActive: b.IsActive,
            CreatedAt: b.CreatedAt
        )).ToList();

        // 5. Encapsulamento de Paginação e Resultado de Sucesso
        var pagedResult = new PagedResult<BrandOutput>(
            items: outputList,
            pageNumber: input.PageNumber,
            pageSize: input.PageSize,
            totalCount: totalCount);

        return Result.Success(pagedResult);
    }
}
