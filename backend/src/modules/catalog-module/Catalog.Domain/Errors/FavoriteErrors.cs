using Shared.Application.Models;

namespace Catalog.Domain.Errors;

/// <summary>
/// Erros relacionados a produtos favoritos.
/// </summary>
public static class FavoriteErrors
{
    public static Error NotFound(Guid favoriteId) =>
        Error.NotFound("UserFavorite", favoriteId);

    public static Error NotFoundByUserAndProduct(Guid userId, Guid productId) =>
        Error.NotFound("UserFavorite.NotFound", 
            $"O produto {productId} não está nos favoritos do usuário {userId}.");

    public static Error AlreadyFavorited(Guid productId) =>
        Error.Conflict("UserFavorite", 
            $"O produto {productId} já está nos seus favoritos.");

    public static Error ProductNotFound(Guid productId) =>
        Error.NotFound("UserFavorite.ProductNotFound", 
            $"O produto {productId} não foi encontrado para adicionar aos favoritos.");

    public static Error MaxFavoritesReached(int maxFavorites) =>
        Error.Failure("UserFavorite.MaxFavoritesReached", 
            $"Você atingiu o limite máximo de {maxFavorites} produtos favoritos.");
}
