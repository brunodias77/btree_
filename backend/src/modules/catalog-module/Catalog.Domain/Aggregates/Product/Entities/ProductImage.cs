using Shared.Domain.Abstractions;

namespace Catalog.Domain.Aggregates.Product.Entities;

/// <summary>
/// Entidade representando uma imagem de produto.
/// </summary>
public sealed class ProductImage : Entity<Guid>
{
    /// <summary>
    /// ID do produto ao qual a imagem pertence.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// URL principal da imagem.
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// Texto alternativo para acessibilidade (alt text).
    /// </summary>
    public string? AltText { get; private set; }

    /// <summary>
    /// URL da imagem em tamanho thumbnail.
    /// </summary>
    public string? UrlThumbnail { get; private set; }

    /// <summary>
    /// URL da imagem em tamanho médio.
    /// </summary>
    public string? UrlMedium { get; private set; }

    /// <summary>
    /// URL da imagem em tamanho grande.
    /// </summary>
    public string? UrlLarge { get; private set; }

    /// <summary>
    /// Indica se é a imagem principal do produto.
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// Ordem de exibição da imagem.
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Construtor privado para EF Core.
    /// </summary>
    private ProductImage() : base() { }

    /// <summary>
    /// Construtor privado para criação da imagem.
    /// </summary>
    private ProductImage(
        Guid id,
        Guid productId,
        string url,
        string? altText = null,
        bool isPrimary = false,
        int sortOrder = 0) : base(id)
    {
        ProductId = productId;
        Url = url;
        AltText = altText;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
    }

    /// <summary>
    /// Factory method para criar uma nova imagem de produto.
    /// </summary>
    public static ProductImage Create(
        Guid productId,
        string url,
        string? altText = null,
        bool isPrimary = false,
        int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("A URL da imagem é obrigatória.", nameof(url));
        }

        return new ProductImage(Guid.NewGuid(), productId, url, altText, isPrimary, sortOrder);
    }

    /// <summary>
    /// Define as URLs otimizadas da imagem.
    /// </summary>
    public void SetOptimizedUrls(string? thumbnail, string? medium, string? large)
    {
        UrlThumbnail = thumbnail;
        UrlMedium = medium;
        UrlLarge = large;
    }

    /// <summary>
    /// Atualiza o texto alternativo.
    /// </summary>
    public void UpdateAltText(string? altText)
    {
        AltText = altText;
    }

    /// <summary>
    /// Define como imagem principal.
    /// </summary>
    public void SetAsPrimary()
    {
        IsPrimary = true;
    }

    /// <summary>
    /// Remove como imagem principal.
    /// </summary>
    public void UnsetAsPrimary()
    {
        IsPrimary = false;
    }

    /// <summary>
    /// Atualiza a ordem de exibição.
    /// </summary>
    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }
}
