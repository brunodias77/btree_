using System.Text.RegularExpressions;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Value Object representando um código de barras de produto.
/// </summary>
public sealed partial class Barcode : ValueObject
{
    /// <summary>
    /// Comprimento máximo do código de barras.
    /// </summary>
    public const int MaxLength = 50;

    /// <summary>
    /// Valor do código de barras.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Tipo do código de barras (EAN-13, EAN-8, UPC-A, etc.).
    /// </summary>
    public BarcodeType Type { get; }

    private Barcode(string value, BarcodeType type)
    {
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Cria um novo código de barras.
    /// </summary>
    public static Barcode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("O código de barras não pode ser vazio.", nameof(value));

        var normalized = value.Trim().Replace(" ", "").Replace("-", "");

        if (normalized.Length > MaxLength)
            throw new ArgumentException($"O código de barras não pode ter mais de {MaxLength} caracteres.", nameof(value));

        if (!OnlyDigitsRegex().IsMatch(normalized))
            throw new ArgumentException("O código de barras deve conter apenas dígitos.", nameof(value));

        var type = DetectType(normalized);

        return new Barcode(normalized, type);
    }

    /// <summary>
    /// Tenta criar um código de barras, retornando null se inválido.
    /// </summary>
    public static Barcode? TryCreate(string? value)
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

    private static BarcodeType DetectType(string value) => value.Length switch
    {
        8 => BarcodeType.EAN8,
        12 => BarcodeType.UPCA,
        13 => BarcodeType.EAN13,
        14 => BarcodeType.GTIN14,
        _ => BarcodeType.Other
    };

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Barcode barcode) => barcode.Value;

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex OnlyDigitsRegex();
}

/// <summary>
/// Tipos de código de barras.
/// </summary>
public enum BarcodeType
{
    EAN8,
    EAN13,
    UPCA,
    GTIN14,
    Other
}
