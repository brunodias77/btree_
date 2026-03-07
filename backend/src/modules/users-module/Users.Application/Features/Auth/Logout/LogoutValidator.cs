using FluentValidation;

namespace Users.Application.Features.Auth.Logout;

public class LogoutValidator : AbstractValidator<LogoutInput>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("O Refresh Token é obrigatório para realizar o logout.")
            .NotNull().WithMessage("O Refresh Token é obrigatório para realizar o logout.");
    }
}
