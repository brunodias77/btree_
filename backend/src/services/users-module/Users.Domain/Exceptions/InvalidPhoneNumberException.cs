using BuildingBlocks.Domain.Exceptions;

namespace Users.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um número de telefone é inválido.
/// </summary>
public sealed class InvalidPhoneNumberException : DomainException
{
    public string? PhoneNumber { get; }

    public InvalidPhoneNumberException(string? phoneNumber)
        : base("O número de telefone informado é inválido.", "PhoneNumber.Invalido")
    {
        PhoneNumber = phoneNumber;
    }

    public InvalidPhoneNumberException(string? phoneNumber, string message)
        : base(message, "PhoneNumber.Invalido")
    {
        PhoneNumber = phoneNumber;
    }

    public static InvalidPhoneNumberException ForEmptyValue() =>
        new(null, "O número de telefone não pode ser vazio.");

    public static InvalidPhoneNumberException ForInvalidFormat(string phoneNumber) =>
        new(phoneNumber, "O número de telefone deve conter entre 10 e 11 dígitos.");
}
