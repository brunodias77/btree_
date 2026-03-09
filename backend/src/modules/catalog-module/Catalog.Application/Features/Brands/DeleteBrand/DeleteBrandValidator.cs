using FluentValidation;

namespace Catalog.Application.Features.Brands.DeleteBrand;

public class DeleteBrandValidator : AbstractValidator<DeleteBrandInput>
{
    public DeleteBrandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador da marca é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O identificador da marca não pode ser vazio.");
    }
}
