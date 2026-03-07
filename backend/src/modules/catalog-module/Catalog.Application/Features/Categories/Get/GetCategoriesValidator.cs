using FluentValidation;

namespace Catalog.Application.Features.Categories.Get;

public class GetCategoriesValidator : AbstractValidator<GetCategoriesInput>
{
    public GetCategoriesValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("O tamanho da página deve ser maior ou igual a 1.")
            .LessThanOrEqualTo(100).WithMessage("O tamanho da página não pode exceder 100 itens.");

        RuleFor(x => x.ParentId)
            .NotEqual(Guid.Empty).WithMessage("O ID da categoria pai não pode ser um Guid vazio.")
            .When(x => x.ParentId.HasValue);
    }
}
