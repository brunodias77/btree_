using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Categories.Get;

public interface IGetCategoriesUseCase : IUseCase<GetCategoriesInput, PagedResult<CategoryOutput>>
{
}
