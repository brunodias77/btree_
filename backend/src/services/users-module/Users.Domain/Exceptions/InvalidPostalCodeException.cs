using BuildingBlocks.Domain.Exceptions;

namespace Users.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um CEP é inválido.
/// </summary>
public sealed class InvalidPostalCodeException : DomainException
{
    public string? PostalCode { get; }

    public InvalidPostalCodeException(string? postalCode)
        : base("O CEP informado é inválido.", "PostalCode.Invalido")
    {
        PostalCode = postalCode;
    }

    public InvalidPostalCodeException(string? postalCode, string message)
        : base(message, "PostalCode.Invalido")
    {
        PostalCode = postalCode;
    }

    public static InvalidPostalCodeException ForEmptyValue() =>
        new(null, "O CEP não pode ser vazio.");

    public static InvalidPostalCodeException ForInvalidFormat(string postalCode) =>
        new(postalCode, "O CEP deve conter 8 dígitos.");
}
