using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Brands.Commands.UpdateBrand;

public interface IUpdateBrandUseCase : IUseCase<UpdateBrandInput, Guid>
{
}
