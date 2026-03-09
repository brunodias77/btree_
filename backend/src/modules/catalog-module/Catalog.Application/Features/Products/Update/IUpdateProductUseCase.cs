using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Products.Update;

public interface IUpdateProductUseCase : IUseCase<UpdateProductInput, Guid>
{
}
