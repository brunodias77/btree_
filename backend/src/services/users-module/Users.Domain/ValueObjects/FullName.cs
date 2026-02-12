using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Application.Models;
using Users.Domain.Errors;

namespace Users.Domain.ValueObjects;

/// <summary>
/// Value object representing a person's full name.
/// </summary>
public sealed class FullName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public static Result<FullName> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<FullName>(ProfileErrors.NomeObrigatorio);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<FullName>(ProfileErrors.SobrenomeObrigatorio);

        if (firstName.Trim().Length > 100)
             return Result.Failure<FullName>(Error.Validation("Profile.NomeMuitoLongo", "O nome não pode exceder 100 caracteres."));

        if (lastName.Trim().Length > 100)
             return Result.Failure<FullName>(Error.Validation("Profile.SobrenomeMuitoLongo", "O sobrenome não pode exceder 100 caracteres."));

        return Result.Success(new FullName(firstName, lastName));
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public string GetInitials() => $"{FirstName[0]}{LastName[0]}".ToUpperInvariant();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName.ToLowerInvariant();
        yield return LastName.ToLowerInvariant();
    }

    public override string ToString() => GetFullName();
}
