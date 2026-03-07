namespace Users.Application.Features.Auth.RefreshToken;

public record RefreshTokenInput(
    string RefreshToken,
    string? IpAddress,
    string? DeviceInfo
);
