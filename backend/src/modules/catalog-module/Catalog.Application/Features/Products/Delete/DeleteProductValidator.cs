using FluentValidation;

namespace Catalog.Application.Features.Products.Delete;

public class DeleteProductValidator : AbstractValidator<DeleteProductInput>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID do produto é obrigatório.");
    }
}
