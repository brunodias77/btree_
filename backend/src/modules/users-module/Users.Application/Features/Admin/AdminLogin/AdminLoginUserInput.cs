using Users.Domain.Enums;

namespace Users.Application.Features.Admin.AdminLogin;

public record AdminLoginUserInput(
    string Email,
    string Password,
    string IpAddress,
    string DeviceName,
    DeviceType DeviceType
);
