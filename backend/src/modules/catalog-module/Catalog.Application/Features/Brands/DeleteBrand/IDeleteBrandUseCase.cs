using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Catalog.Application.Features.Brands.Commands.DeleteBrand;

public interface IDeleteBrandUseCase : IUseCase<DeleteBrandInput, Result>
{
}
