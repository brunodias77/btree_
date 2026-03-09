
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Brands.CreateBrand;

public interface ICreateBrandUseCase : IUseCase<CreateBrandInput, Guid>
{
}
