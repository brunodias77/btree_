namespace Users.Application.Features.Admin.AdminLogin;

public record AdminLoginUserOutput(    Guid UserId,
    string Email,
    string? FirstName,
    string? LastName,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);