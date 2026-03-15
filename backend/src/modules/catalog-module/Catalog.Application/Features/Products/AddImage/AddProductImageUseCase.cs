using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Catalog.Domain.Services;
using FluentValidation;
using Shared.Application.Models;

namespace Catalog.Application.Features.Products.AddImage;

public class AddProductImageUseCase : IAddProductImageUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IValidator<AddProductImageInput> _validator;

    public AddProductImageUseCase(
        IProductRepository productRepository,
        IImageStorageService imageStorageService,
        ICatalogUnitOfWork unitOfWork,
        IValidator<AddProductImageInput> validator)
    {
        _productRepository = productRepository;
        _imageStorageService = imageStorageService;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<string>> ExecuteAsync(AddProductImageInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validacao de Campos Base
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Obter Produto
        var product = await _productRepository.GetByIdAsync(input.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure<string>(ProductErrors.NotFound(input.ProductId));
        }

        // Limite de Imagens (verificação preventiva para caso o domínio restrinja sem erro explicito)
        if (product.Images.Count >= 10)
        {
            return Result.Failure<string>(ImageErrors.MaxImagesExceeded);
        }

        try
        {
            // Gerar o novo nome do arquivo: product-01.*, product-02.*, etc.
            var fileExtension = System.IO.Path.GetExtension(input.FileName);
            var nextImageNumber = product.Images.Count + 1;
            var newFileName = $"product-{nextImageNumber:D2}{fileExtension}";

            // 3. Upload da Imagem
            var imageUrl = await _imageStorageService.UploadImageAsync(
                newFileName,
                input.ImageStream,
                input.ContentType,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
               return Result.Failure<string>(ImageErrors.UploadFailed);
            }

            // 4. Criação dos Value Objects e Atualização do Domínio
            // Observação: O método `product.AddImage()` cria a entidade filha `ProductImage` e atualiza as flags `IsPrimary`.
            // O Value Object `ImageUrl` e `ProductImage` são criados internamente pelo agregado `Product` ou instanciados como strings diretas no dominio de acordo com a modelagem do dominio verificada (string url em product.AddImage).
            product.AddImage(url: imageUrl, altText: input.FileName, isPrimary: input.IsPrimary);

            // 5. Persistência
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Retorno
            return Result.Success(imageUrl);
        }
        catch (Exception ex)
        {
            // Falha inesperada (ex: erro no Storage Service ou de banco)
            return Result.Failure<string>(ImageErrors.UploadError(ex.Message));
        }
    }
}
