using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Brands.DeleteBrand;

public interface IDeleteBrandUseCase : IUseCase<DeleteBrandInput, Result>
{
}
