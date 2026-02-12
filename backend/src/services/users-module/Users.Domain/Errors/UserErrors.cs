using BuildingBlocks.Application.Models;

namespace Users.Domain.Errors;

public static class UserErrors
{
    public static readonly Error EmailNotUnique = Error.Conflict(
        "User.EmailNotUnique",
        "O email informado já está em uso.");

    public static readonly Error NotFound = Error.NotFound(
        "User.NotFound",
        "Usuário não encontrado.");
        
    public static readonly Error RegistrationFailed = Error.Failure(
        "User.RegistrationFailed",
        "Falha ao registrar o usuário.");
}
