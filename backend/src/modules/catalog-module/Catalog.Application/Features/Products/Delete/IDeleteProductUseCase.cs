using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Products.Delete;

public interface IDeleteProductUseCase : IUseCase<DeleteProductInput, Result>
{
}
