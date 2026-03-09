using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Brands.UpdateBrand;

public interface IUpdateBrandUseCase : IUseCase<UpdateBrandInput, Guid>
{
}
