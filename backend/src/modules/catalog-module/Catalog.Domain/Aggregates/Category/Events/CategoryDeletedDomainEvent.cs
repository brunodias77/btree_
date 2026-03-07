using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Category.Events;

/// <summary>
/// Evento de domínio disparado quando uma categoria é excluída (soft delete).
/// </summary>
/// <param name="CategoryId">ID da categoria excluída.</param>
/// <param name="Name">Nome da categoria excluída.</param>
public sealed class CategoryDeletedDomainEvent : DomainEventBase
{
    public Guid CategoryId { get; }
    public string Name { get; }

    public override string AggregateType => nameof(Category);
    public override Guid AggregateId => CategoryId;
    public override string Module => "catalog";

    public CategoryDeletedDomainEvent(Guid categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
    }
}


