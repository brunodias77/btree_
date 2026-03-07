using FluentValidation;

namespace Catalog.Application.Features.Categories.Update;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryInput>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador da categoria é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da categoria não pode exceder 100 caracteres.");

        RuleFor(x => x.Slug)
            .MaximumLength(200).WithMessage("O slug não pode exceder 200 caracteres.")
            .Matches(@"^[a-z0-9\-]+$").WithMessage("O slug deve conter apenas letras minúsculas, números e hífens.")
            .When(x => !string.IsNullOrEmpty(x.Slug));

        RuleFor(x => x.MetaTitle)
            .MaximumLength(70).WithMessage("O título SEO não pode exceder 70 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(160).WithMessage("A descrição SEO não pode exceder 160 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("A ordem de exibição deve ser maior ou igual a zero.");
    }
}
