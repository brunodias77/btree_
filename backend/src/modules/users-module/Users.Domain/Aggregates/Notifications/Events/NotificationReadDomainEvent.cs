using Shared.Domain.Events;

namespace Users.Domain.Aggregates.Notifications.Events;

public class NotificationReadDomainEvent : DomainEventBase
{
    public override string AggregateType => nameof(Notification);
    public override Guid AggregateId { get; }
    public override string Module => "users";
    public Guid UserId { get; }
    
    public NotificationReadDomainEvent(Guid aggregateId, Guid userId)
    {
        AggregateId = aggregateId;
        UserId = userId;
    }
}