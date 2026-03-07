namespace Users.Application.Features.Auth.RefreshToken;

public record RefreshTokenOutput(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
