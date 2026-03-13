using Shared.Application.UseCases;

namespace Catalog.Application.Features.Products.RemoveImage;

public interface IRemoveProductImageUseCase : IUseCase<RemoveProductImageInput, bool>
{
}
