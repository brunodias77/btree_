using Shared.Application.Events;

namespace Shared.IntegrationEvents.Events.Users;

public class UserRegisteredEvent : IIntegrationEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string AggregateType { get; }
    public Guid AggregateId { get; }
    public string Module { get; }
}