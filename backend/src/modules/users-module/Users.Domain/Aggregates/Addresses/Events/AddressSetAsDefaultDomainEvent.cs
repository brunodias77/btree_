using Shared.Domain.Events;

namespace Users.Domain.Aggregates.Addresses.Events;

public class AddressSetAsDefaultDomainEvent : DomainEventBase
{
    public override string AggregateType => nameof(Address);
    public override Guid AggregateId { get; }
    public override string Module => "users";
    public Guid UserId { get; }

    public AddressSetAsDefaultDomainEvent(Guid aggregateId, Guid userId)
    {
        AggregateId = aggregateId;
        UserId = userId;
    }
}