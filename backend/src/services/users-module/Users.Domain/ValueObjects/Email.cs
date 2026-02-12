using System.Text.RegularExpressions;
using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.ValueObjects;

/// <summary>
/// Value object representing an email address.
/// </summary>
public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value.ToLowerInvariant().Trim();
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        email = email.Trim();

        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("Invalid email format.", nameof(email));

        return new Email(email);
    }

    public static Email? TryCreate(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return EmailRegex.IsMatch(email.Trim()) ? new Email(email) : null;
    }

    public static bool IsValid(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
