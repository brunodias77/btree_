using System.Text.RegularExpressions;
using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.ValueObjects;

/// <summary>
/// Value object representing a Brazilian postal code (CEP).
/// </summary>
public sealed class PostalCode : ValueObject
{
    private static readonly Regex PostalCodeRegex = new(
        @"^\d{5}-?\d{3}$",
        RegexOptions.Compiled);

    public string Value { get; }
    public string Formatted { get; }

    private PostalCode(string value)
    {
        var digits = new string(value.Where(char.IsDigit).ToArray());
        Value = digits;
        Formatted = $"{digits[..5]}-{digits[5..]}";
    }

    public static PostalCode Create(string postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty.", nameof(postalCode));

        var digits = new string(postalCode.Where(char.IsDigit).ToArray());

        if (digits.Length != 8)
            throw new ArgumentException("Postal code must have 8 digits.", nameof(postalCode));

        return new PostalCode(digits);
    }

    public static PostalCode? TryCreate(string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return null;

        var digits = new string(postalCode.Where(char.IsDigit).ToArray());
        
        if (digits.Length != 8)
            return null;

        return new PostalCode(digits);
    }

    public static bool IsValid(string postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return false;

        var digits = new string(postalCode.Where(char.IsDigit).ToArray());
        return digits.Length == 8;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Formatted;

    public static implicit operator string(PostalCode postalCode) => postalCode.Value;
}
