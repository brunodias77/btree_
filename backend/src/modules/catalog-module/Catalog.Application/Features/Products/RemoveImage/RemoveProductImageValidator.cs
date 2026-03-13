using FluentValidation;

namespace Catalog.Application.Features.Products.RemoveImage;

public class RemoveProductImageValidator : AbstractValidator<RemoveProductImageInput>
{
    public RemoveProductImageValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("O ID do produto é obrigatório.");

        RuleFor(x => x.ImageId)
            .NotEmpty().WithMessage("O ID da imagem é obrigatório.");
    }
}
