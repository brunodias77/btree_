using FluentValidation;

namespace Catalog.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandValidator : AbstractValidator<CreateBrandInput>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da marca é obrigatório.")
            .Length(2, 100).WithMessage("O nome da marca deve ter entre 2 e 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("A descrição não pode exceder 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.LogoUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("A URL do logo deve ser válida e absoluta (http/https).")
            .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));
    }
}
