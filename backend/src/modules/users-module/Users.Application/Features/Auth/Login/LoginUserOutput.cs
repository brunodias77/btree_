namespace Users.Application.Features.Auth.Login;

public record LoginUserOutput(
    Guid UserId,
    string Email,
    string? FirstName,
    string? LastName,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);