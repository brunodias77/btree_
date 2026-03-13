using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Catalog.Domain.Services;
using FluentValidation;
using Shared.Application.Models;

namespace Catalog.Application.Features.Products.RemoveImage;

public class RemoveProductImageUseCase : IRemoveProductImageUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IValidator<RemoveProductImageInput> _validator;

    public RemoveProductImageUseCase(
        IProductRepository productRepository,
        IImageStorageService imageStorageService,
        ICatalogUnitOfWork unitOfWork,
        IValidator<RemoveProductImageInput> validator)
    {
        _productRepository = productRepository;
        _imageStorageService = imageStorageService;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<bool>> ExecuteAsync(RemoveProductImageInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validacao de Campos Base
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<bool>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Obter Produto
        var product = await _productRepository.GetByIdAsync(input.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure<bool>(ProductErrors.NotFound(input.ProductId));
        }

        // 3. Identificar Imagem que será removida
        var imageToRemove = product.Images.FirstOrDefault(i => i.Id == input.ImageId);
        if (imageToRemove is null)
        {
            return Result.Failure<bool>(ImageErrors.NotFound);
        }

        // Guarda a URL antes da exclusão no domínio.
        var imageUrl = imageToRemove.Url;

        // 4. Atualização do Domínio
        product.RemoveImage(input.ImageId);

        try
        {
            // 5. Exclusão no Storage
            await _imageStorageService.DeleteImageAsync(imageUrl, cancellationToken);
        }
        catch (Exception)
        {
            // Falha não crítica, ou podemos reportar dependendo da regra de negócio.
            // Aqui decidimos seguir o Fail explícito se a deleção falhar e se importar.
            return Result.Failure<bool>(ImageErrors.DeletionFailed);
        }

        // 6. Persistência
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retorno
        return Result.Success(true);
    }
}
