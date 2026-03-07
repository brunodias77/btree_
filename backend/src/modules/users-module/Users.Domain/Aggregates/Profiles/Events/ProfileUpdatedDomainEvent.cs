using Shared.Domain.Events;

namespace Users.Domain.Aggregates.Profiles.Events;

public class ProfileUpdatedDomainEvent : DomainEventBase
{
    public override string AggregateType => nameof(Profile);
    public override Guid AggregateId { get; }
    public override string Module => "users";
    public Guid UserId { get; }

    public ProfileUpdatedDomainEvent(Guid aggregateId, Guid userId)
    {
        AggregateId = aggregateId;
        UserId = userId;
    }
}