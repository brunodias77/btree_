using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Categories.Update;

public interface IUpdateCategoryUseCase : IUseCase<UpdateCategoryInput, Guid>
{
}
