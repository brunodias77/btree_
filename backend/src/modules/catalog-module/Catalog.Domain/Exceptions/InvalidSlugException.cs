
using Shared.Domain.Exceptions;

namespace Catalog.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um Slug é inválido.
/// </summary>
public class InvalidSlugException : DomainException
{
    public string Slug { get; }

    public InvalidSlugException(string slug)
        : base("INVALID_SLUG", $"O slug '{slug}' é inválido. O slug deve conter apenas letras minúsculas, números e hífens.")
    {
        Slug = slug;
    }

    public InvalidSlugException(string slug, string reason)
        : base("INVALID_SLUG", $"O slug '{slug}' é inválido: {reason}")
    {
        Slug = slug;
    }
}
