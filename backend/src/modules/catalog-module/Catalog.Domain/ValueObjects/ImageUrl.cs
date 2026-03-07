using Shared.Domain.Abstractions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Value Object representando uma URL de imagem.
/// </summary>
public sealed class ImageUrl : ValueObject
{
    /// <summary>
    /// URL original da imagem.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// URL da thumbnail (opcional).
    /// </summary>
    public string? ThumbnailUrl { get; }

    /// <summary>
    /// URL em tamanho médio (opcional).
    /// </summary>
    public string? MediumUrl { get; }

    /// <summary>
    /// URL em tamanho grande (opcional).
    /// </summary>
    public string? LargeUrl { get; }

    /// <summary>
    /// Texto alternativo para acessibilidade.
    /// </summary>
    public string? AltText { get; }

    private ImageUrl(string url, string? thumbnailUrl, string? mediumUrl, string? largeUrl, string? altText)
    {
        Url = url;
        ThumbnailUrl = thumbnailUrl;
        MediumUrl = mediumUrl;
        LargeUrl = largeUrl;
        AltText = altText;
    }

    /// <summary>
    /// Cria uma nova URL de imagem.
    /// </summary>
    public static ImageUrl Create(
        string url,
        string? altText = null,
        string? thumbnailUrl = null,
        string? mediumUrl = null,
        string? largeUrl = null)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("A URL da imagem não pode ser vazia.", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("A URL da imagem é inválida.", nameof(url));

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("A URL deve usar HTTP ou HTTPS.", nameof(url));

        return new ImageUrl(url, thumbnailUrl, mediumUrl, largeUrl, altText);
    }

    /// <summary>
    /// Cria uma URL de imagem apenas com a URL principal.
    /// </summary>
    public static ImageUrl FromUrl(string url) => Create(url);

    /// <summary>
    /// Adiciona URLs otimizadas.
    /// </summary>
    public ImageUrl WithOptimizedUrls(string? thumbnailUrl, string? mediumUrl, string? largeUrl)
    {
        return new ImageUrl(Url, thumbnailUrl, mediumUrl, largeUrl, AltText);
    }

    /// <summary>
    /// Adiciona texto alternativo.
    /// </summary>
    public ImageUrl WithAltText(string altText)
    {
        return new ImageUrl(Url, ThumbnailUrl, MediumUrl, LargeUrl, altText);
    }

    /// <summary>
    /// Retorna a URL mais apropriada para o tamanho solicitado.
    /// </summary>
    public string GetUrl(ImageSize size = ImageSize.Original) => size switch
    {
        ImageSize.Thumbnail => ThumbnailUrl ?? Url,
        ImageSize.Medium => MediumUrl ?? Url,
        ImageSize.Large => LargeUrl ?? Url,
        _ => Url
    };

    /// <summary>
    /// Verifica se tem URLs otimizadas.
    /// </summary>
    public bool HasOptimizedUrls => ThumbnailUrl != null || MediumUrl != null || LargeUrl != null;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Url;
        yield return ThumbnailUrl;
        yield return MediumUrl;
        yield return LargeUrl;
        yield return AltText;
    }

    public override string ToString() => Url;

    public static implicit operator string(ImageUrl imageUrl) => imageUrl.Url;
}

/// <summary>
/// Tamanhos de imagem disponíveis.
/// </summary>
public enum ImageSize
{
    Original,
    Thumbnail,
    Medium,
    Large
}
