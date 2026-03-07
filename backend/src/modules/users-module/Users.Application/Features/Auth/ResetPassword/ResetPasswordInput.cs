namespace Users.Application.Features.Auth.ResetPassword;

public record ResetPasswordInput(
    string Code,
    string NewPassword);
