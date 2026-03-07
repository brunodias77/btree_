namespace Users.Application.Features.Auth.ChangePassword;

public record ChangePasswordInput(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword);
