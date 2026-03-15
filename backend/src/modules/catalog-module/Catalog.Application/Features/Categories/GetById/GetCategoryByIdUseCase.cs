using Catalog.Application.Features.Categories.Get;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Shared.Application.Models;

namespace Catalog.Application.Features.Categories.GetById;

public class GetCategoryByIdUseCase : IGetCategoryByIdUseCase
{
    private readonly ICategoryReadRepository _readRepository;

    public GetCategoryByIdUseCase(ICategoryReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<Result<CategoryOutput>> ExecuteAsync(GetCategoryByIdInput input, CancellationToken cancellationToken = default)
    {
        var category = await _readRepository.GetByIdAsync(input.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure<CategoryOutput>(CategoryErrors.NotFound(input.Id));
        }

        var output = new CategoryOutput(
            Id: category.Id,
            Name: category.Name,
            Slug: category.Slug,
            ParentId: category.ParentId,
            Path: category.Path,
            IsActive: category.IsActive,
            CreatedAt: category.CreatedAt
        );

        return Result.Success(output);
    }
}
