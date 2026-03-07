using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Categories.Delete;

public interface IDeleteCategoryUseCase : IUseCase<DeleteCategoryInput, Guid>
{
}
