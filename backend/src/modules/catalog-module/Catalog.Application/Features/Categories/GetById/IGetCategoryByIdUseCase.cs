using Catalog.Application.Features.Categories.Get;
using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Categories.GetById;

public interface IGetCategoryByIdUseCase : IUseCase<GetCategoryByIdInput, CategoryOutput>
{
}
