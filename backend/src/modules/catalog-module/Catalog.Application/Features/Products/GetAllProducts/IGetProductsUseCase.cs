using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Products.GetAllProducts;

public interface IGetProductsUseCase : IUseCase<GetProductsInput, PagedResult<ProductOutput>>
{
}
