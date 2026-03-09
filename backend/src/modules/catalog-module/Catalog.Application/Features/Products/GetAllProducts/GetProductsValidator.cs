using FluentValidation;

namespace Catalog.Application.Features.Products.GetAllProducts;

public class GetProductsValidator : AbstractValidator<GetProductsInput>
{
    private static readonly string[] AllowedSortFields = { "Name", "Price", "CreatedAt", "Stock" };

    public GetProductsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve ser entre 1 e 100.");

        When(x => !string.IsNullOrEmpty(x.SortBy), () =>
        {
            RuleFor(x => x.SortBy)
                .Must(field => AllowedSortFields.Contains(field, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Campo de ordenação inválido. Permitidos: {string.Join(", ", AllowedSortFields)}.");
        });

        When(x => !string.IsNullOrEmpty(x.SortDirection), () =>
        {
            RuleFor(x => x.SortDirection)
                .Must(dir => dir.Equals("asc", StringComparison.OrdinalIgnoreCase) || 
                             dir.Equals("desc", StringComparison.OrdinalIgnoreCase) ||
                             dir.Equals("ascending", StringComparison.OrdinalIgnoreCase) ||
                             dir.Equals("descending", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Direção de ordenação inválida. Use 'asc' ou 'desc'.");
        });
    }
}
