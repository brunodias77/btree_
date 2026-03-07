using Shared.Domain.Events;

namespace Users.Domain.Aggregates.Sessions.Events;

public class SessionCreatedDomainEvent : DomainEventBase
{
    public override string AggregateType => nameof(Session);
    public override Guid AggregateId { get; }
    public override string Module => "users";

    public Guid UserId { get; }
    public string? DeviceType { get; }
    public string? IpAddress { get; }

    public SessionCreatedDomainEvent(Guid aggregateId, Guid userId, string? deviceType = null, string? ipAddress = null)
    {
        AggregateId = aggregateId;
        UserId = userId;
        DeviceType = deviceType;
        IpAddress = ipAddress;
    }
}