using System.Text.RegularExpressions;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Value Object representando um SKU (Stock Keeping Unit).
/// SKU é um código único para identificar produtos no estoque.
/// </summary>
public sealed partial class Sku : ValueObject
{
    /// <summary>
    /// Comprimento máximo do SKU.
    /// </summary>
    public const int MaxLength = 100;

    /// <summary>
    /// Valor do SKU.
    /// </summary>
    public string Value { get; }

    private Sku(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria um novo SKU validado.
    /// </summary>
    /// <param name="value">Valor do SKU.</param>
    /// <returns>Nova instância de Sku.</returns>
    /// <exception cref="ArgumentException">Quando o valor é inválido.</exception>
    public static Sku Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("O SKU não pode ser vazio.", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length > MaxLength)
            throw new ArgumentException($"O SKU não pode ter mais de {MaxLength} caracteres.", nameof(value));

        if (!SkuRegex().IsMatch(normalized))
            throw new ArgumentException("O SKU deve conter apenas letras, números, hífens e underscores.", nameof(value));

        return new Sku(normalized);
    }

    /// <summary>
    /// Tenta criar um SKU, retornando null se inválido.
    /// </summary>
    public static Sku? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        try
        {
            return Create(value);
        }
        catch
        {
            return null;
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Sku sku) => sku.Value;

    [GeneratedRegex(@"^[A-Z0-9\-_]+$")]
    private static partial Regex SkuRegex();
}
