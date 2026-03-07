namespace Catalog.Domain.Services;

/// <summary>
/// Serviço para geração de slugs URL-friendly.
/// </summary>
public interface ISlugGenerator
{
    string GenerateSlug(string text);
}
