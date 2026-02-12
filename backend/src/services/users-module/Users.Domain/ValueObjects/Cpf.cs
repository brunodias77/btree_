using System.Text.RegularExpressions;
using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.ValueObjects;

/// <summary>
/// Value object representing a Brazilian CPF (Cadastro de Pessoas Físicas).
/// </summary>
public sealed class Cpf : ValueObject
{
    private static readonly Regex CpfFormattedRegex = new(
        @"^\d{3}\.\d{3}\.\d{3}-\d{2}$",
        RegexOptions.Compiled);

    public string Value { get; }
    public string Formatted { get; }

    private Cpf(string value)
    {
        var digits = new string(value.Where(char.IsDigit).ToArray());
        Value = digits;
        Formatted = $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..]}";
    }

    public static Cpf Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF cannot be empty.", nameof(cpf));

        var digits = new string(cpf.Where(char.IsDigit).ToArray());

        if (digits.Length != 11)
            throw new ArgumentException("CPF must have 11 digits.", nameof(cpf));

        if (!IsValidCpf(digits))
            throw new ArgumentException("Invalid CPF.", nameof(cpf));

        return new Cpf(digits);
    }

    public static Cpf? TryCreate(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return null;

        var digits = new string(cpf.Where(char.IsDigit).ToArray());
        
        if (digits.Length != 11 || !IsValidCpf(digits))
            return null;

        return new Cpf(digits);
    }

    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var digits = new string(cpf.Where(char.IsDigit).ToArray());
        return digits.Length == 11 && IsValidCpf(digits);
    }

    private static bool IsValidCpf(string cpf)
    {
        // Check for known invalid sequences
        if (cpf.Distinct().Count() == 1)
            return false;

        // Validate first check digit
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (cpf[i] - '0') * (10 - i);

        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        if (cpf[9] - '0' != firstDigit)
            return false;

        // Validate second check digit
        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += (cpf[i] - '0') * (11 - i);

        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return cpf[10] - '0' == secondDigit;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Formatted;

    public static implicit operator string(Cpf cpf) => cpf.Value;
}
