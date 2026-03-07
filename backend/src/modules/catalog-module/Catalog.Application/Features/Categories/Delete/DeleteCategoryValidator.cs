using FluentValidation;

namespace Catalog.Application.Features.Categories.Delete;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryInput>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador da categoria é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O identificador da categoria não pode ser vazio.");
    }
}
