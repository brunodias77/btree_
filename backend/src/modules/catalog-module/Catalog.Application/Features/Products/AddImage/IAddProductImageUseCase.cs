using Shared.Application.UseCases;

namespace Catalog.Application.Features.Products.AddImage;

public interface IAddProductImageUseCase : IUseCase<AddProductImageInput, string>
{
}
