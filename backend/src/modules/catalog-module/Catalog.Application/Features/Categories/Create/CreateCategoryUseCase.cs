using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using Catalog.Domain.ValueObjects;
using Shared.Application.Models;
using CategoryEntity = Catalog.Domain.Aggregates.Category.Category;

namespace Catalog.Application.Features.Categories.Create;

public class CreateCategoryUseCase : ICreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly FluentValidation.IValidator<CreateCategoryInput> _validator;

    public CreateCategoryUseCase(
        ICategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork,
        FluentValidation.IValidator<CreateCategoryInput> validator)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> ExecuteAsync(CreateCategoryInput input, CancellationToken cancellationToken = default)
    {
        // 0. Validar Input
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 1. Tratamento do Slug
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

        // 2. Verificação de Unicidade do Slug
        var slugExists = await _categoryRepository.ExistsBySlugAsync(slug.Value, cancellationToken);
        if (slugExists)
        {
            return Result.Failure<Guid>(CategoryErrors.SlugAlreadyExists(slug.Value));
        }

        // 3. Resolução da Hierarquia (Parent)
        Guid? parentId = input.ParentId;
        string? parentPath = null;
        int parentDepth = -1;

        if (parentId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(parentId.Value, cancellationToken);

            if (parentCategory is null)
            {
                return Result.Failure<Guid>(CategoryErrors.NotFound(parentId.Value));
            }

            parentPath = parentCategory.Path;
            parentDepth = parentCategory.Depth;

            // Verificar preventivamente se a profundidade do pai já atingiu o limite
            if (parentDepth >= 5)
            {
                return Result.Failure<Guid>(CategoryErrors.MaxDepthExceeded(5));
            }
        }

        // 4. Criação da Entidade via Factory Method
        var category = CategoryEntity.Create(
            name: input.Name,
            slug: slug.Value,
            parentId: parentId,
            parentPath: parentPath,
            parentDepth: parentDepth);

        // 5. Enriquecimento da Entidade com atributos opcionais
        category.Update(
            name: input.Name,
            slug: slug.Value,
            description: input.Description,
            imageUrl: input.ImageUrl,
            metaTitle: input.MetaTitle,
            metaDescription: input.MetaDescription,
            sortOrder: input.SortOrder);

        // 6. Persistência
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retorno
        return Result.Success(category.Id);
    }
}
