using FluentValidation;

namespace Users.Application.Features.Auth.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordInput>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("A senha atual é obrigatória.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("A nova senha é obrigatória.")
            .MinimumLength(8).WithMessage("A nova senha deve ter no mínimo 8 caracteres.")
            .Matches("[A-Z]").WithMessage("A nova senha deve conter pelo menos uma letra maiúscula.")
            .Matches("[0-9]").WithMessage("A nova senha deve conter pelo menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("A nova senha deve conter pelo menos um caractere especial.")
            .NotEqual(x => x.CurrentPassword).WithMessage("A nova senha não pode ser igual à senha atual.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword).WithMessage("A confirmação da nova senha não confere.");
    }
}
