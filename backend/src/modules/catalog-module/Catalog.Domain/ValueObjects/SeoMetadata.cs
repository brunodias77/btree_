using Shared.Domain.Abstractions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Value Object representando metadados SEO de um produto ou categoria.
/// </summary>
public sealed class SeoMetadata : ValueObject
{
    /// <summary>
    /// Comprimento máximo do título SEO.
    /// </summary>
    public const int MaxTitleLength = 70;

    /// <summary>
    /// Comprimento máximo da descrição SEO.
    /// </summary>
    public const int MaxDescriptionLength = 160;

    /// <summary>
    /// Título para SEO (meta title).
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// Descrição para SEO (meta description).
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Palavras-chave para SEO (opcional).
    /// </summary>
    public IReadOnlyList<string> Keywords { get; }

    private SeoMetadata(string? title, string? description, IReadOnlyList<string> keywords)
    {
        Title = title;
        Description = description;
        Keywords = keywords;
    }

    /// <summary>
    /// Cria metadados SEO.
    /// </summary>
    public static SeoMetadata Create(
        string? title = null,
        string? description = null,
        IEnumerable<string>? keywords = null)
    {
        if (title != null && title.Length > MaxTitleLength)
            throw new ArgumentException($"O título SEO não pode ter mais de {MaxTitleLength} caracteres.", nameof(title));

        if (description != null && description.Length > MaxDescriptionLength)
            throw new ArgumentException($"A descrição SEO não pode ter mais de {MaxDescriptionLength} caracteres.", nameof(description));

        var keywordsList = keywords?
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => k.Trim().ToLowerInvariant())
            .Distinct()
            .ToList()
            .AsReadOnly() ?? new List<string>().AsReadOnly();

        return new SeoMetadata(title?.Trim(), description?.Trim(), keywordsList);
    }

    /// <summary>
    /// Cria metadados SEO vazios.
    /// </summary>
    public static SeoMetadata Empty() => new(null, null, new List<string>().AsReadOnly());

    /// <summary>
    /// Gera metadados a partir de um nome e descrição de produto.
    /// </summary>
    public static SeoMetadata FromProduct(string productName, string? productDescription)
    {
        var title = productName.Length > MaxTitleLength 
            ? productName[..(MaxTitleLength - 3)] + "..." 
            : productName;

        string? description = null;
        if (!string.IsNullOrWhiteSpace(productDescription))
        {
            description = productDescription.Length > MaxDescriptionLength
                ? productDescription[..(MaxDescriptionLength - 3)] + "..."
                : productDescription;
        }

        return new SeoMetadata(title, description, new List<string>().AsReadOnly());
    }

    /// <summary>
    /// Adiciona título.
    /// </summary>
    public SeoMetadata WithTitle(string title)
    {
        return Create(title, Description, Keywords);
    }

    /// <summary>
    /// Adiciona descrição.
    /// </summary>
    public SeoMetadata WithDescription(string description)
    {
        return Create(Title, description, Keywords);
    }

    /// <summary>
    /// Adiciona palavras-chave.
    /// </summary>
    public SeoMetadata WithKeywords(IEnumerable<string> keywords)
    {
        return Create(Title, Description, keywords);
    }

    /// <summary>
    /// Verifica se está completo (tem título e descrição).
    /// </summary>
    public bool IsComplete => !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Description);

    /// <summary>
    /// Verifica se está vazio.
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Description) && Keywords.Count == 0;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Title;
        yield return Description;
        foreach (var keyword in Keywords)
        {
            yield return keyword;
        }
    }

    public override string ToString()
    {
        if (IsEmpty)
            return "Sem metadados SEO";

        return $"Title: {Title ?? "(vazio)"} | Description: {Description?.Substring(0, Math.Min(30, Description?.Length ?? 0)) ?? "(vazio)"}...";
    }
}
