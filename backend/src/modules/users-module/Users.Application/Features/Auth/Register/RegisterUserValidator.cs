using FluentValidation;

namespace Users.Application.Features.Auth.Register;

public class RegisterUserValidator : AbstractValidator<RegisterUserInput>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .EmailAddress().WithMessage("O email deve ser válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("A senha deve conter pelo menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter pelo menos um caractere especial.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(50).WithMessage("O nome não pode exceder 50 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("O sobrenome é obrigatório.")
            .MaximumLength(50).WithMessage("O sobrenome não pode exceder 50 caracteres.");

        RuleFor(x => x.Cpf)
            .Matches(@"^\d{11}$").WithMessage("O CPF deve conter 11 dígitos.")
            .When(x => !string.IsNullOrEmpty(x.Cpf));

        RuleFor(x => x.BirthDate)
            .LessThan(DateOnly.FromDateTime(DateTime.Today)).WithMessage("A data de nascimento deve ser no passado.")
            .When(x => x.BirthDate.HasValue);
    }
}
