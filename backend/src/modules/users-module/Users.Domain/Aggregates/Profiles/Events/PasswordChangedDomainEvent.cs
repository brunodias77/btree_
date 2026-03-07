using Shared.Domain.Events;

namespace Users.Domain.Aggregates.Profiles.Events;

public class PasswordChangedDomainEvent : DomainEventBase
{
    public override string AggregateType => nameof(Profile);
    public override Guid AggregateId { get; }
    public override string Module => "users";
    
    public Guid UserId { get; }

    public PasswordChangedDomainEvent(Guid aggregateId, Guid userId)
    {
        AggregateId = aggregateId;
        UserId = userId;
    }
}
