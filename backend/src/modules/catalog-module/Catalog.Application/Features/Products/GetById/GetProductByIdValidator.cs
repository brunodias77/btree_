using FluentValidation;

namespace Catalog.Application.Features.Products.GetById;

/// <summary>
/// Valida os dados de entrada para a busca de um produto por ID.
/// </summary>
public class GetProductByIdValidator : AbstractValidator<GetProductByIdInput>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("O ID do produto é obrigatório e deve ser válido.");
    }
}
