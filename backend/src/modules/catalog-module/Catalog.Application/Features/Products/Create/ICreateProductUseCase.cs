using Shared.Application.UseCases;

namespace Catalog.Application.Features.Products.Create;

public interface ICreateProductUseCase : IUseCase<CreateProductInput, Guid>
{
}
