using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.Brand.Events;

/// <summary>
/// Evento de domínio disparado quando uma nova marca é criada.
/// </summary>
/// <param name="BrandId">ID da marca criada.</param>
/// <param name="Name">Nome da marca.</param>
/// <param name="Slug">Slug da marca.</param>
public sealed class BrandCreatedDomainEvent : DomainEventBase
{
    public Guid BrandId { get; }
    public string Name { get; }
    public string Slug { get; }

    public override string AggregateType => nameof(Brand);
    public override Guid AggregateId => BrandId;
    public override string Module => "catalog";

    public BrandCreatedDomainEvent(Guid brandId, string name, string slug)
    {
        BrandId = brandId;
        Name = name;
        Slug = slug;
    }
}

