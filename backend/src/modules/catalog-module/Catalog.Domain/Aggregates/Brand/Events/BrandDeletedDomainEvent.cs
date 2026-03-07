using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Brand.Events;

/// <summary>
/// Evento de domínio disparado quando uma marca é excluída (soft delete).
/// </summary>
/// <param name="BrandId">ID da marca excluída.</param>
/// <param name="Name">Nome da marca excluída.</param>
public sealed class BrandDeletedDomainEvent : DomainEventBase
{
    public Guid BrandId { get; }
    public string Name { get; }

    public override string AggregateType => nameof(Brand);
    public override Guid AggregateId => BrandId;
    public override string Module => "catalog";

    public BrandDeletedDomainEvent(Guid brandId, string name)
    {
        BrandId = brandId;
        Name = name;
    }
}


