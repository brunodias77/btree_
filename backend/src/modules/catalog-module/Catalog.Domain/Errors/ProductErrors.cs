
using Shared.Application.Models;

namespace Catalog.Domain.Errors;

/// <summary>
/// Erros relacionados a produtos.
/// </summary>
public static class ProductErrors
{
    public static Error NotFound(Guid productId) =>
        Error.NotFound("Product", productId);

    public static Error NotFoundBySku(string sku) =>
        Error.NotFound("Product.NotFoundBySku", $"Produto com SKU '{sku}' não foi encontrado.");

    public static Error NotFoundBySlug(string slug) =>
        Error.NotFound("Product.NotFoundBySlug", $"Produto com slug '{slug}' não foi encontrado.");

    public static Error SkuRequired =>
        Error.Validation("Product.Sku", "O SKU do produto é obrigatório.");

    public static Error NameRequired =>
        Error.Validation("Product.Name", "O nome do produto é obrigatório.");

    public static Error InvalidPrice =>
        Error.Validation("Product.Price", "O preço do produto deve ser maior ou igual a zero.");

    public static Error InvalidCompareAtPrice =>
        Error.Validation("Product.CompareAtPrice", "O preço de comparação deve ser maior que o preço de venda.");

    public static Error SkuAlreadyExists(string sku) =>
        Error.Conflict("Product", $"Já existe um produto com o SKU '{sku}'.");

    public static Error SlugAlreadyExists(string slug) =>
        Error.Conflict("Product", $"Já existe um produto com o slug '{slug}'.");

    public static Error Inactive =>
        Error.Failure("Product.Inactive", "Este produto está inativo e não pode ser comprado.");

    public static Error Discontinued =>
        Error.Failure("Product.Discontinued", "Este produto foi descontinuado.");

    public static Error CannotPublishWithoutStock =>
        Error.Failure("Product.CannotPublishWithoutStock", "Não é possível publicar um produto sem estoque.");

    public static Error CannotPublishWithoutImages =>
        Error.Failure("Product.CannotPublishWithoutImages", "Não é possível publicar um produto sem imagens.");

    public static Error CannotDeleteWithActiveReservations =>
        Error.Conflict("Product.ActiveReservations", "Não é possível excluir um produto com reservas de estoque ativas.");
}
