using Shared.Application.Models;

namespace Catalog.Domain.Errors;

/// <summary>
/// Erros relacionados a categorias.
/// </summary>
public static class CategoryErrors
{
    public static Error NotFound(Guid categoryId) =>
        Error.NotFound("Category", categoryId);

    public static Error NotFoundBySlug(string slug) =>
        Error.NotFound("Category.NotFoundBySlug", $"Categoria com slug '{slug}' não foi encontrada.");

    public static Error NameRequired =>
        Error.Validation("Category.Name", "O nome da categoria é obrigatório.");

    public static Error SlugRequired =>
        Error.Validation("Category.Slug", "O slug da categoria é obrigatório.");

    public static Error SlugAlreadyExists(string slug) =>
        Error.Conflict("Category", $"Já existe uma categoria com o slug '{slug}'.");

    public static Error MaxDepthExceeded(int maxDepth) =>
        Error.Failure("Category.MaxDepthExceeded", $"A profundidade máxima de {maxDepth} níveis foi excedida.");

    public static Error CannotDeleteWithChildren =>
        Error.Failure("Category.CannotDeleteWithChildren", "Não é possível excluir uma categoria que possui subcategorias.");

    public static Error CannotDeleteWithProducts =>
        Error.Failure("Category.CannotDeleteWithProducts", "Não é possível excluir uma categoria que possui produtos associados.");

    public static Error CircularReference =>
        Error.Failure("Category.CircularReference", "Não é possível mover uma categoria para dentro de si mesma ou de suas subcategorias.");
}
