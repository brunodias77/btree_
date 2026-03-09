using FluentValidation;

namespace Catalog.Application.Features.Products.Create;

public class CreateProductValidator : AbstractValidator<CreateProductInput>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .Length(3, 150).WithMessage("O nome do produto deve ter entre 3 e 150 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(2000).WithMessage("A descrição não pode exceder 2000 caracteres.");

        RuleFor(x => x.BrandId)
            .NotEmpty().WithMessage("A marca do produto é obrigatória.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("A categoria do produto é obrigatória.");

        RuleFor(x => x.PriceAmount)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

        RuleFor(x => x.PriceCurrency)
            .NotEmpty().WithMessage("A moeda é obrigatória.")
            .Length(3).WithMessage("A moeda deve ter exatamente 3 caracteres.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("O SKU é obrigatório.")
            .Must(x => !x.Contains(" ")).WithMessage("O SKU não pode conter espaços em branco.");

        RuleFor(x => x.InitialStock)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque inicial deve ser maior ou igual a zero.");

        // Dimensões interdependentes
        When(x => x.WeightInGrams.HasValue || x.LengthInCm.HasValue || x.WidthInCm.HasValue || x.HeightInCm.HasValue, () =>
        {
            RuleFor(x => x.WeightInGrams)
                .NotNull().WithMessage("Se dimensões foram fornecidas, o peso é obrigatório.")
                .GreaterThan(0).WithMessage("O peso deve ser maior que zero.");

            RuleFor(x => x.LengthInCm)
                .NotNull().WithMessage("Se dimensões foram fornecidas, o comprimento é obrigatório.")
                .GreaterThan(0).WithMessage("O comprimento deve ser maior que zero.");

            RuleFor(x => x.WidthInCm)
                .NotNull().WithMessage("Se dimensões foram fornecidas, a largura é obrigatória.")
                .GreaterThan(0).WithMessage("A largura deve ser maior que zero.");

            RuleFor(x => x.HeightInCm)
                .NotNull().WithMessage("Se dimensões foram fornecidas, a altura é obrigatória.")
                .GreaterThan(0).WithMessage("A altura deve ser maior que zero.");
        });
    }
}
