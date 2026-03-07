using System.Text.RegularExpressions;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Value Object representando um Slug para URLs amigáveis.
/// </summary>
public sealed partial class Slug : ValueObject
{
    /// <summary>
    /// Comprimento máximo do Slug.
    /// </summary>
    public const int MaxLength = 200;

    /// <summary>
    /// Valor do Slug.
    /// </summary>
    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria um novo Slug validado.
    /// </summary>
    /// <param name="value">Valor do Slug.</param>
    /// <returns>Nova instância de Slug.</returns>
    /// <exception cref="ArgumentException">Quando o valor é inválido.</exception>
    public static Slug Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("O slug não pode ser vazio.", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
            throw new ArgumentException($"O slug não pode ter mais de {MaxLength} caracteres.", nameof(value));

        if (!SlugRegex().IsMatch(normalized))
            throw new ArgumentException("O slug deve conter apenas letras minúsculas, números e hífens.", nameof(value));

        return new Slug(normalized);
    }

    /// <summary>
    /// Gera um slug a partir de um texto.
    /// </summary>
    /// <param name="text">Texto para gerar o slug.</param>
    /// <returns>Slug gerado.</returns>
    public static Slug FromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("O texto não pode ser vazio.", nameof(text));

        var slug = text.Trim().ToLowerInvariant();
        
        // Remove acentos
        slug = RemoveAccents(slug);
        
        // Substitui espaços e caracteres não-alfanuméricos por hífens
        slug = NonAlphanumericRegex().Replace(slug, "-");
        
        // Remove hífens duplicados
        slug = MultipleHyphensRegex().Replace(slug, "-");
        
        // Remove hífens do início e fim
        slug = slug.Trim('-');

        if (slug.Length > MaxLength)
            slug = slug[..MaxLength].TrimEnd('-');

        return new Slug(slug);
    }

    private static string RemoveAccents(string text)
    {
        var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
        var builder = new System.Text.StringBuilder();

        foreach (var c in normalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != 
                System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Slug slug) => slug.Value;

    [GeneratedRegex(@"^[a-z0-9\-]+$")]
    private static partial Regex SlugRegex();

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonAlphanumericRegex();

    [GeneratedRegex(@"-+")]
    private static partial Regex MultipleHyphensRegex();
}
