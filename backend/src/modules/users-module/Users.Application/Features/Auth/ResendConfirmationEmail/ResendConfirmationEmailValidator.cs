using FluentValidation;

namespace Users.Application.Features.Auth.ResendConfirmationEmail;

public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailInput>
{
    public ResendConfirmationEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O formato do e-mail é inválido ou não foi fornecido.")
            .NotNull().WithMessage("O formato do e-mail é inválido ou não foi fornecido.")
            .EmailAddress().WithMessage("O formato do e-mail é inválido ou não foi fornecido.");
    }
}
