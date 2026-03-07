using FluentValidation;

namespace Users.Application.Features.Auth.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailInput>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("O código de confirmação é obrigatório.")
            .Length(6).WithMessage("O código de confirmação deve ter 6 dígitos.")
            .Matches(@"^\d+$").WithMessage("O código de confirmação deve conter apenas números.");
    }
}
