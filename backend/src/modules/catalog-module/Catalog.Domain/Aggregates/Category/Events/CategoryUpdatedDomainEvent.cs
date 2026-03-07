using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Category.Events;

/// <summary>
/// Evento de domínio disparado quando uma categoria é atualizada.
/// </summary>
/// <param name="CategoryId">ID da categoria atualizada.</param>
/// <param name="Name">Novo nome da categoria.</param>
/// <param name="Slug">Novo slug da categoria.</param>
public sealed class CategoryUpdatedDomainEvent : DomainEventBase
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public string Slug { get; }

    public override string AggregateType => nameof(Category);
    public override Guid AggregateId => CategoryId;
    public override string Module => "catalog";

    public CategoryUpdatedDomainEvent(Guid categoryId, string name, string slug)
    {
        CategoryId = categoryId;
        Name = name;
        Slug = slug;
    }
}


