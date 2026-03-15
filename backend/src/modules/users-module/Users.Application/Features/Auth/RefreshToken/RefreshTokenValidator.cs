using FluentValidation;

namespace Users.Application.Features.Auth.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenInput>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("O Refresh Token é obrigatório.")
            .NotNull().WithMessage("O Refresh Token é obrigatório.");

        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("O endereço IP é obrigatório.");
    }
}
