namespace Users.Application.Features.Auth.Register;

public record RegisterUserInput(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Cpf,
    DateOnly? BirthDate);