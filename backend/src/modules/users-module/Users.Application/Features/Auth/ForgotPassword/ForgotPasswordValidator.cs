using FluentValidation;

namespace Users.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordInput>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O formato do e-mail é inválido ou não foi fornecido.")
            .NotNull().WithMessage("O formato do e-mail é inválido ou não foi fornecido.")
            .EmailAddress().WithMessage("O formato do e-mail é inválido ou não foi fornecido.");
    }
}
