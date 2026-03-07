using Catalog.Domain.Aggregates.Category.Events;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.Aggregates.Category;

/// <summary>
/// Aggregate Root representando uma categoria do catálogo.
/// Categorias podem ser hierárquicas (pai/filho) com limite de profundidade de 5 níveis.
/// </summary>
public sealed class Category : AggregateRoot<Guid>
{
    /// <summary>
    /// ID da categoria pai (null se for categoria raiz).
    /// </summary>
    public Guid? ParentId { get; private set; }

    /// <summary>
    /// Caminho hierárquico da categoria (ex: "/eletronicos/celulares/smartphones").
    /// </summary>
    public string? Path { get; private set; }

    /// <summary>
    /// Profundidade na hierarquia (0 = raiz, 1 = primeiro nível, etc.).
    /// Máximo permitido: 5.
    /// </summary>
    public int Depth { get; private set; }

    /// <summary>
    /// Nome da categoria.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Slug da categoria para URL amigável.
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada da categoria.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// URL da imagem da categoria.
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Título para SEO (máx. 70 caracteres).
    /// </summary>
    public string? MetaTitle { get; private set; }

    /// <summary>
    /// Descrição para SEO (máx. 160 caracteres).
    /// </summary>
    public string? MetaDescription { get; private set; }

    /// <summary>
    /// Indica se a categoria está ativa.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Ordem de exibição da categoria.
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Construtor privado para EF Core.
    /// </summary>
    private Category() : base() { }

    /// <summary>
    /// Construtor privado para criação da categoria.
    /// </summary>
    private Category(
        Guid id,
        string name,
        string slug,
        Guid? parentId = null,
        string? path = null,
        int depth = 0) : base(id)
    {
        Name = name;
        Slug = slug;
        ParentId = parentId;
        Path = path;
        Depth = depth;
    }

    /// <summary>
    /// Factory method para criar uma nova categoria.
    /// </summary>
    /// <param name="name">Nome da categoria.</param>
    /// <param name="slug">Slug para URL.</param>
    /// <param name="parentId">ID da categoria pai (opcional).</param>
    /// <param name="parentPath">Caminho da categoria pai (opcional).</param>
    /// <param name="parentDepth">Profundidade da categoria pai (opcional).</param>
    /// <returns>Nova instância de Category.</returns>
    /// <exception cref="InvalidOperationException">Quando a profundidade excede o limite.</exception>
    public static Category Create(
        string name,
        string slug,
        Guid? parentId = null,
        string? parentPath = null,
        int parentDepth = -1)
    {
        var depth = parentId.HasValue ? parentDepth + 1 : 0;
        
        if (depth > 5)
        {
            throw new InvalidOperationException("A profundidade máxima da categoria (5 níveis) foi excedida.");
        }

        var id = Guid.NewGuid();
        var path = parentPath != null ? $"{parentPath}/{slug}" : $"/{slug}";

        var category = new Category(id, name, slug, parentId, path, depth);
        
        category.RegisterDomainEvent(new CategoryCreatedDomainEvent(
            category.Id,
            category.Name,
            category.Slug,
            category.ParentId));

        return category;
    }

    /// <summary>
    /// Atualiza os dados da categoria.
    /// </summary>
    /// <param name="name">Novo nome.</param>
    /// <param name="slug">Novo slug.</param>
    /// <param name="description">Nova descrição.</param>
    /// <param name="imageUrl">Nova URL da imagem.</param>
    /// <param name="metaTitle">Novo título SEO.</param>
    /// <param name="metaDescription">Nova descrição SEO.</param>
    /// <param name="sortOrder">Nova ordem de exibição.</param>
    public void Update(
        string name,
        string slug,
        string? description = null,
        string? imageUrl = null,
        string? metaTitle = null,
        string? metaDescription = null,
        int sortOrder = 0)
    {
        // Recalcular Path se o Slug mudou
        if (Slug != slug)
        {
            // Extrair o caminho do pai (tudo antes do último segmento)
            var parentPath = Path != null && Path.Contains('/')
                ? Path[..Path.LastIndexOf('/')]
                : string.Empty;
            
            // Se parentPath está vazio, significa que é raiz
            Path = string.IsNullOrEmpty(parentPath) ? $"/{slug}" : $"{parentPath}/{slug}";
        }

        Name = name;
        Slug = slug;
        Description = description;
        ImageUrl = imageUrl;
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        SortOrder = sortOrder;
        IncrementVersion();

        RegisterDomainEvent(new CategoryUpdatedDomainEvent(Id, Name, Slug));
    }

    /// <summary>
    /// Move a categoria para um novo pai.
    /// </summary>
    /// <param name="newParentId">ID do novo pai (null para mover para raiz).</param>
    /// <param name="newParentPath">Caminho do novo pai.</param>
    /// <param name="newParentDepth">Profundidade do novo pai.</param>
    /// <exception cref="InvalidOperationException">Quando a nova profundidade excede o limite.</exception>
    public void MoveTo(Guid? newParentId, string? newParentPath, int newParentDepth = -1)
    {
        var oldParentId = ParentId;
        var newDepth = newParentId.HasValue ? newParentDepth + 1 : 0;

        if (newDepth > 5)
        {
            throw new InvalidOperationException("A profundidade máxima da categoria (5 níveis) foi excedida.");
        }

        ParentId = newParentId;
        Path = newParentPath != null ? $"{newParentPath}/{Slug}" : $"/{Slug}";
        Depth = newDepth;
        IncrementVersion();

        RegisterDomainEvent(new CategoryMovedDomainEvent(Id, oldParentId, newParentId));
    }

    /// <summary>
    /// Ativa a categoria.
    /// </summary>
    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            IncrementVersion();
        }
    }

    /// <summary>
    /// Desativa a categoria.
    /// </summary>
    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            IncrementVersion();
        }
    }

    /// <summary>
    /// Marca a categoria como excluída (soft delete) e dispara evento de domínio.
    /// </summary>
    public override void Delete()
    {
        if (IsDeleted) return;

        base.Delete();
        IsActive = false;
        IncrementVersion();

        RegisterDomainEvent(new CategoryDeletedDomainEvent(Id, Name));
    }

    /// <summary>
    /// Restaura uma categoria excluída.
    /// </summary>
    public override void Restore()
    {
        if (!IsDeleted) return;

        base.Restore();
        IsActive = true;
        IncrementVersion();
    }
}
