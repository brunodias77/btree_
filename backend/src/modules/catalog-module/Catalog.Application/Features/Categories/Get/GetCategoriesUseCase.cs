using Catalog.Domain.Repositories;
using Shared.Application.Models;

namespace Catalog.Application.Features.Categories.Get;

public class GetCategoriesUseCase : IGetCategoriesUseCase
{
    private readonly ICategoryReadRepository _readRepository;
    private readonly FluentValidation.IValidator<GetCategoriesInput> _validator;

    public GetCategoriesUseCase(
        ICategoryReadRepository readRepository,
        FluentValidation.IValidator<GetCategoriesInput> validator)
    {
        _readRepository = readRepository;
        _validator = validator;
    }

    public async Task<Result<PagedResult<CategoryOutput>>> ExecuteAsync(GetCategoriesInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validação do DTO de entrada
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PagedResult<CategoryOutput>>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Consulta de Leitura otimizada no Repositório
        var (items, totalCount) = await _readRepository.BrowseAsync(
            searchTerm: input.SearchTerm,
            parentId: input.ParentId,
            isActive: input.IsActive,
            page: input.PageNumber,
            pageSize: input.PageSize,
            cancellationToken: cancellationToken);

        // 3. Early return em caso de array vazio
        if (totalCount == 0)
        {
            return Result.Success(PagedResult<CategoryOutput>.Empty(input.PageSize));
        }

        // 4. Mapeamento
        var outputList = items.Select(c => new CategoryOutput(
            Id: c.Id,
            Name: c.Name,
            Slug: c.Slug,
            ParentId: c.ParentId,
            Path: c.Path,
            IsActive: c.IsActive,
            CreatedAt: c.CreatedAt
        )).ToList();

        // 5. Encapsulamento de Paginação e Resultado de Sucesso
        var pagedResult = new PagedResult<CategoryOutput>(
            items: outputList,
            pageNumber: input.PageNumber,
            pageSize: input.PageSize,
            totalCount: totalCount);

        return Result.Success(pagedResult);
    }
}
