using System.Text.RegularExpressions;
using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Application.Models;
using Users.Domain.Errors;

namespace Users.Domain.ValueObjects;

/// <summary>
/// Value object representing a Brazilian phone number.
/// </summary>
public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex PhoneRegex = new(
        @"^\+?55?\s?\(?\d{2}\)?\s?\d{4,5}[-\s]?\d{4}$",
        RegexOptions.Compiled);

    public string Value { get; }
    public string? CountryCode { get; }
    public string? AreaCode { get; }
    public string? Number { get; }

    private PhoneNumber(string value)
    {
        Value = NormalizePhoneNumber(value);

        // Extract components
        var digits = new string(Value.Where(char.IsDigit).ToArray());
        if (digits.Length >= 10)
        {
            CountryCode = "55";
            AreaCode = digits.Substring(digits.Length - 11, 2);
            Number = digits.Substring(digits.Length - 9);
        }
    }

    public static Result<PhoneNumber> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure<PhoneNumber>(Error.Validation("Profile.TelefoneObrigatorio", "O telefone é obrigatório."));

        if (!IsValid(phoneNumber))
            return Result.Failure<PhoneNumber>(ProfileErrors.TelefoneInvalido);

        return Result.Success(new PhoneNumber(phoneNumber));
    }

    public static bool IsValid(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        return digits.Length >= 10 && digits.Length <= 13;
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

        if (digits.Length == 11)
            return $"+55 ({digits[..2]}) {digits[2..7]}-{digits[7..]}";

        if (digits.Length == 10)
            return $"+55 ({digits[..2]}) {digits[2..6]}-{digits[6..]}";

        return phoneNumber;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;
}
