using Users.Domain.Enums;

namespace Users.Application.Features.Auth.Login;

public record LoginUserInput(    string Email,
    string Password,
    string? IpAddress,
    string? DeviceName,
    DeviceType? DeviceType);