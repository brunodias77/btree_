using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Category.Events;

/// <summary>
/// Evento de domínio disparado quando uma nova categoria é criada.
/// </summary>
/// <param name="CategoryId">ID da categoria criada.</param>
/// <param name="Name">Nome da categoria.</param>
/// <param name="Slug">Slug da categoria.</param>
/// <param name="ParentId">ID da categoria pai (null se for raiz).</param>
public sealed class CategoryCreatedDomainEvent : DomainEventBase
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public string Slug { get; }
    public Guid? ParentId { get; }

    public override string AggregateType => nameof(Category);
    public override Guid AggregateId => CategoryId;
    public override string Module => "catalog";

    public CategoryCreatedDomainEvent(Guid categoryId, string name, string slug, Guid? parentId)
    {
        CategoryId = categoryId;
        Name = name;
        Slug = slug;
        ParentId = parentId;
    }
}


