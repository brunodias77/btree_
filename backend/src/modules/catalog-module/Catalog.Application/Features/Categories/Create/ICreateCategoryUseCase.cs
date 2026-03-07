using Shared.Application.UseCases;

namespace Catalog.Application.Features.Categories.Create;

public interface ICreateCategoryUseCase : IUseCase<CreateCategoryInput, Guid>
{
    
}
