using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Brands.GetBrands;

public interface IGetBrandsUseCase : IUseCase<GetBrandsInput, PagedResult<BrandOutput>>
{
}
