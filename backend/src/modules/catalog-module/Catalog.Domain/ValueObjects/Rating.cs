using Shared.Domain.Abstractions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Value Object representando uma nota de avaliação (1 a 5 estrelas).
/// </summary>
public sealed class Rating : ValueObject
{
    /// <summary>
    /// Valor mínimo da nota.
    /// </summary>
    public const int MinValue = 1;

    /// <summary>
    /// Valor máximo da nota.
    /// </summary>
    public const int MaxValue = 5;

    /// <summary>
    /// Valor da nota.
    /// </summary>
    public int Value { get; }

    private Rating(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria uma nova nota validada.
    /// </summary>
    /// <param name="value">Valor da nota (1 a 5).</param>
    /// <returns>Nova instância de Rating.</returns>
    /// <exception cref="ArgumentException">Quando o valor é inválido.</exception>
    public static Rating Create(int value)
    {
        if (value < MinValue || value > MaxValue)
            throw new ArgumentException($"A nota deve estar entre {MinValue} e {MaxValue}.", nameof(value));

        return new Rating(value);
    }

    /// <summary>
    /// Calcula a média de várias notas.
    /// </summary>
    public static decimal Average(IEnumerable<Rating> ratings)
    {
        var list = ratings.ToList();
        
        if (list.Count == 0)
            return 0;

        return Math.Round((decimal)list.Sum(r => r.Value) / list.Count, 2);
    }

    /// <summary>
    /// Cria uma nota a partir de uma média calculada.
    /// </summary>
    public static Rating FromAverage(decimal average)
    {
        var rounded = (int)Math.Round(average);
        rounded = Math.Clamp(rounded, MinValue, MaxValue);
        return new Rating(rounded);
    }

    /// <summary>
    /// Verifica se é uma nota positiva (4 ou 5).
    /// </summary>
    public bool IsPositive => Value >= 4;

    /// <summary>
    /// Verifica se é uma nota negativa (1 ou 2).
    /// </summary>
    public bool IsNegative => Value <= 2;

    /// <summary>
    /// Verifica se é uma nota neutra (3).
    /// </summary>
    public bool IsNeutral => Value == 3;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => $"{Value}/5";

    public static implicit operator int(Rating rating) => rating.Value;
}
