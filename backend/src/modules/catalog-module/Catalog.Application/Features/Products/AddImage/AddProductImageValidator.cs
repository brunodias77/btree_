using FluentValidation;

namespace Catalog.Application.Features.Products.AddImage;

public class AddProductImageValidator : AbstractValidator<AddProductImageInput>
{
    private static readonly string[] ValidContentTypes = { "image/jpeg", "image/png", "image/webp" };
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5MB

    public AddProductImageValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("O ID do produto é obrigatório.");

        RuleFor(x => x.ImageStream)
            .NotNull().WithMessage("O arquivo de imagem é obrigatório.")
            .Must(stream => stream != null && stream.Length > 0).WithMessage("O arquivo de imagem não pode estar vazio.")
            .Must(stream => stream != null && stream.Length <= MaxFileSizeInBytes).WithMessage($"O tamanho da imagem não pode exceder {MaxFileSizeInBytes / 1024 / 1024}MB.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("O nome do arquivo é obrigatório.")
            .MaximumLength(255).WithMessage("O nome do arquivo não pode exceder 255 caracteres.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("O tipo de conteúdo (MIME type) é obrigatório.")
            .Must(contentType => ValidContentTypes.Contains(contentType.ToLowerInvariant()))
            .WithMessage($"O tipo de arquivo não é suportado. Tipos permitidos: {string.Join(", ", ValidContentTypes)}");
    }
}
