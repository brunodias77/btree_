using BuildingBlocks.Domain.Exceptions;

namespace Users.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um email é inválido.
/// </summary>
public sealed class InvalidEmailException : DomainException
{
    public string Email { get; }

    public InvalidEmailException(string email)
        : base($"O email '{email}' é inválido.", "Email.Invalido")
    {
        Email = email;
    }

    public InvalidEmailException(string email, string message)
        : base(message, "Email.Invalido")
    {
        Email = email;
    }
}
