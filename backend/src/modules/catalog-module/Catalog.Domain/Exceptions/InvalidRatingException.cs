using Shared.Domain.Exceptions;

namespace Catalog.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando uma nota de avaliação é inválida.
/// </summary>
public class InvalidRatingException : DomainException
{
    public int Rating { get; }

    public InvalidRatingException(int rating)
        : base("INVALID_RATING", $"A nota '{rating}' é inválida. A nota deve estar entre 1 e 5.")
    {
        Rating = rating;
    }
}
