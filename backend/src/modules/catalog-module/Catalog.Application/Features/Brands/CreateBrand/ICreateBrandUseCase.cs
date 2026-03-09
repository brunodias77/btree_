using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Brands.Commands.CreateBrand;

public interface ICreateBrandUseCase : IUseCase<CreateBrandInput, Guid>
{
}
