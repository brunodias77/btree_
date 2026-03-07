using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Catalog.Domain.ValueObjects;
using Shared.Application.Models;

namespace Catalog.Application.Features.Categories.Update;

public class UpdateCategoryUseCase : IUpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly FluentValidation.IValidator<UpdateCategoryInput> _validator;

    public UpdateCategoryUseCase(
        ICategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork,
        FluentValidation.IValidator<UpdateCategoryInput> validator)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> ExecuteAsync(UpdateCategoryInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validar Input
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Recuperação da Entidade
        var category = await _categoryRepository.GetByIdAsync(input.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure<Guid>(CategoryErrors.NotFound(input.Id));
        }

        // 3. Tratamento do Slug
        Slug slug;
        try
        {
            slug = !string.IsNullOrWhiteSpace(input.Slug)
                ? Slug.Create(input.Slug)
                : Slug.FromText(input.Name);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(new Error("Category.InvalidSlug", ex.Message));
        }

        // 4. Verificação de Unicidade de Slug (somente se mudou)
        if (category.Slug != slug.Value)
        {
            var slugExists = await _categoryRepository.ExistsBySlugExcludingAsync(slug.Value, category.Id, cancellationToken);
            if (slugExists)
            {
                return Result.Failure<Guid>(CategoryErrors.SlugAlreadyExists(slug.Value));
            }
        }

        // 5. Atualização do Agregado
        category.Update(
            name: input.Name,
            slug: slug.Value,
            description: input.Description,
            imageUrl: input.ImageUrl,
            metaTitle: input.MetaTitle,
            metaDescription: input.MetaDescription,
            sortOrder: input.SortOrder);

        // 6. Persistência
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retorno
        return Result.Success(category.Id);
    }
}
