using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Shared.Application.Models;

namespace Catalog.Application.Features.Categories.Delete;

public class DeleteCategoryUseCase : IDeleteCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly FluentValidation.IValidator<DeleteCategoryInput> _validator;

    public DeleteCategoryUseCase(
        ICategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork,
        FluentValidation.IValidator<DeleteCategoryInput> validator)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> ExecuteAsync(DeleteCategoryInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validar Input
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Recuperação da Entidade e Validação de Inexistência
        var category = await _categoryRepository.GetByIdAsync(input.Id, cancellationToken);
        if (category is null || category.IsDeleted)
        {
            return Result.Failure<Guid>(CategoryErrors.NotFound(input.Id));
        }

        // 3. Verificação de Subcategorias
        var hasChildren = await _categoryRepository.HasChildrenAsync(category.Id, cancellationToken);
        if (hasChildren)
        {
            return Result.Failure<Guid>(CategoryErrors.CannotDeleteWithChildren);
        }

        // 4. Verificação de Produtos Associados
        var hasProducts = await _categoryRepository.HasProductsAsync(category.Id, cancellationToken);
        if (hasProducts)
        {
            return Result.Failure<Guid>(CategoryErrors.CannotDeleteWithProducts);
        }

        // 5. Aplicação do Soft Delete
        category.Delete();

        // 6. Persistência
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retorno
        return Result.Success(category.Id);
    }
}
