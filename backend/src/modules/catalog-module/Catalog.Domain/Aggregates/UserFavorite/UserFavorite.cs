
using Catalog.Domain.Aggregates.UserFavorite.Events;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.Aggregates.UserFavorite;

/// <summary>
/// Aggregate Root representando um produto favoritado por um usuário.
/// </summary>
public sealed class UserFavorite : AggregateRoot<Guid>
{
    /// <summary>
    /// ID do usuário que favoritou o produto.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// ID do produto favoritado.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Snapshot dos dados do produto no momento do favorito (JSON).
    /// Útil para mostrar o produto mesmo se ele for excluído.
    /// </summary>
    public string? ProductSnapshot { get; private set; }

    /// <summary>
    /// Construtor privado para EF Core.
    /// </summary>
    private UserFavorite() : base() { }

    /// <summary>
    /// Construtor privado para criação do favorito.
    /// </summary>
    private UserFavorite(Guid id, Guid userId, Guid productId) : base(id)
    {
        UserId = userId;
        ProductId = productId;
    }

    /// <summary>
    /// Factory method para criar um novo favorito.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="productId">ID do produto.</param>
    /// <param name="productSnapshot">Snapshot do produto em JSON (opcional).</param>
    /// <returns>Nova instância de UserFavorite.</returns>
    public static UserFavorite Create(
        Guid userId,
        Guid productId,
        string? productSnapshot = null)
    {
        var favorite = new UserFavorite(Guid.NewGuid(), userId, productId)
        {
            ProductSnapshot = productSnapshot
        };

        favorite.RegisterDomainEvent(new ProductFavoritedDomainEvent(
            favorite.Id,
            favorite.UserId,
            favorite.ProductId));

        return favorite;
    }

    /// <summary>
    /// Atualiza o snapshot do produto.
    /// </summary>
    /// <param name="productSnapshot">Novo snapshot em JSON.</param>
    public void UpdateProductSnapshot(string? productSnapshot)
    {
        ProductSnapshot = productSnapshot;
        IncrementVersion();
    }

    /// <summary>
    /// Remove o favorito (dispara evento de unfavorite).
    /// </summary>
    public void Unfavorite()
    {
        RegisterDomainEvent(new ProductUnfavoritedDomainEvent(
            Id,
            UserId,
            ProductId));
    }
}
