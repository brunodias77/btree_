using Shared.Domain.Events;

namespace Users.Domain.Aggregates.Addresses.Events;

public class AddressCreatedDomainEvent : DomainEventBase
{
    public override string AggregateType => nameof(Address);
    public override Guid AggregateId { get; }
    public override string Module => "users";
    public Guid UserId { get; }
    
    public AddressCreatedDomainEvent(Guid aggregateId, Guid userId)
    {
        AggregateId = aggregateId;
        UserId = userId;
    }
}

