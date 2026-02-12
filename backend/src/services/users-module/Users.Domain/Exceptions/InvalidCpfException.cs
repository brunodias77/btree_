using BuildingBlocks.Domain.Exceptions;

namespace Users.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um CPF é inválido.
/// </summary>
public sealed class InvalidCpfException : DomainException
{
    public string? Cpf { get; }

    public InvalidCpfException(string? cpf)
        : base($"O CPF informado é inválido.", "Cpf.Invalido")
    {
        Cpf = cpf;
    }

    public InvalidCpfException(string? cpf, string message)
        : base(message, "Cpf.Invalido")
    {
        Cpf = cpf;
    }

    public static InvalidCpfException ForInvalidFormat() =>
        new(null, "O CPF deve conter 11 dígitos.");

    public static InvalidCpfException ForInvalidCheckDigits(string cpf) =>
        new(cpf, "O CPF informado possui dígitos verificadores inválidos.");
}
