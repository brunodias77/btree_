using Shared.Application.UseCases;

namespace Catalog.Application.Features.Products.GetById;

/// <summary>
/// Interface para o caso de uso de consulta de produto por ID.
/// </summary>
public interface IGetProductByIdUseCase : IUseCase<GetProductByIdInput, ProductOutput>
{
}
