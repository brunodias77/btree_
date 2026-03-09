using FluentValidation;

namespace Catalog.Application.Features.Brands.GetBrands;

public class GetBrandsValidator : AbstractValidator<GetBrandsInput>
{
    public GetBrandsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("O tamanho da página deve ser maior ou igual a 1.")
            .LessThanOrEqualTo(100).WithMessage("O tamanho da página não pode exceder 100.");
    }
}
