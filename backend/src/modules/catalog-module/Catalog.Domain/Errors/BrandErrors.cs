using Shared.Application.Models;

namespace Catalog.Domain.Errors;

/// <summary>
/// Erros relacionados a marcas.
/// </summary>
public static class BrandErrors
{
    public static Error DuplicateName => 
        Error.Conflict("Brand.DuplicateName", "Já existe uma marca com este nome.");

    public static Error NotFound(Guid brandId) =>
        Error.NotFound("Brand", brandId);

    public static Error NotFoundBySlug(string slug) =>
        Error.NotFound("Brand.NotFoundBySlug", $"Marca com slug '{slug}' não foi encontrada.");

    public static Error NameRequired =>
        Error.Validation("Brand.Name", "O nome da marca é obrigatório.");

    public static Error SlugRequired =>
        Error.Validation("Brand.Slug", "O slug da marca é obrigatório.");

    public static Error SlugAlreadyExists(string slug) =>
        Error.Conflict("Brand", $"Já existe uma marca com o slug '{slug}'.");

    public static Error CannotDeleteWithProducts =>
        Error.Failure("Brand.CannotDeleteWithProducts", "Não é possível excluir uma marca que possui produtos associados.");
}
