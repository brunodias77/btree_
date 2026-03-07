using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Category.Events;

/// <summary>
/// Evento de domínio disparado quando uma categoria é movida para outro pai.
/// </summary>
/// <param name="CategoryId">ID da categoria movida.</param>
/// <param name="OldParentId">ID do pai anterior (null se era raiz).</param>
/// <param name="NewParentId">ID do novo pai (null se movida para raiz).</param>
public sealed class CategoryMovedDomainEvent : DomainEventBase
{
    public Guid CategoryId { get; }
    public Guid? OldParentId { get; }
    public Guid? NewParentId { get; }

    public override string AggregateType => nameof(Category);
    public override Guid AggregateId => CategoryId;
    public override string Module => "catalog";

    public CategoryMovedDomainEvent(Guid categoryId, Guid? oldParentId, Guid? newParentId)
    {
        CategoryId = categoryId;
        OldParentId = oldParentId;
        NewParentId = newParentId;
    }
}


