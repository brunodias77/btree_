using FluentValidation;

namespace Users.Application.Features.Auth.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O formato do e-mail é inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .Matches("[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("A senha deve conter pelo menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter pelo menos um caractere especial.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("O primeiro nome é obrigatório.")
            .MaximumLength(100).WithMessage("O primeiro nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("O sobrenome é obrigatório.")
            .MaximumLength(100).WithMessage("O sobrenome deve ter no máximo 100 caracteres.");

        When(x => !string.IsNullOrEmpty(x.Cpf), () =>
        {
            RuleFor(x => x.Cpf)
                .Matches(@"^\d{11}$|^\d{3}\.\d{3}\.\d{3}-\d{2}$")
                .WithMessage("CPF inválido.");
        });

        When(x => x.BirthDate.HasValue, () =>
        {
            RuleFor(x => x.BirthDate)
                .Must(BeAtLeast18YearsOld)
                .WithMessage("O usuário deve ser maior de 18 anos.");
        });
    }

    private bool BeAtLeast18YearsOld(DateOnly? birthDate)
    {
        if (!birthDate.HasValue) return true;
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Value.Year;
        
        if (birthDate.Value > today.AddYears(-age))
        {
            age--;
        }
        
        return age >= 18;
    }
}