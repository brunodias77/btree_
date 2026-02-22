using BuildingBlocks.Application.Messaging;

namespace Users.Application.Features.Auth.Register;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Cpf,
    DateOnly? BirthDate
) : ICommand<Guid>;