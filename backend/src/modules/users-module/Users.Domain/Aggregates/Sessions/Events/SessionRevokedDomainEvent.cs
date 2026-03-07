using Shared.Domain.Events;

namespace Users.Domain.Aggregates.Sessions.Events;

public class SessionRevokedDomainEvent : DomainEventBase
{
    public override string AggregateType => nameof(Session);
    public override Guid AggregateId { get; }
    public override string Module => "users";

    public Guid UserId { get; }
    public string? Reason { get; }

    public SessionRevokedDomainEvent(Guid aggregateId, Guid userId, string? reason = null)
    {
        AggregateId = aggregateId;
        UserId = userId;
        Reason = reason;
    }
}