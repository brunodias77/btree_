using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Brand.Events;

/// <summary>
/// Evento de domínio disparado quando uma marca é atualizada.
/// </summary>
/// <param name="BrandId">ID da marca atualizada.</param>
/// <param name="Name">Novo nome da marca.</param>
/// <param name="Slug">Novo slug da marca.</param>
public sealed class BrandUpdatedDomainEvent : DomainEventBase
{
    public Guid BrandId { get; }
    public string Name { get; }
    public string Slug { get; }

    public override string AggregateType => nameof(Brand);
    public override Guid AggregateId => BrandId;
    public override string Module => "catalog";

    public BrandUpdatedDomainEvent(Guid brandId, string name, string slug)
    {
        BrandId = brandId;
        Name = name;
        Slug = slug;
    }
}


