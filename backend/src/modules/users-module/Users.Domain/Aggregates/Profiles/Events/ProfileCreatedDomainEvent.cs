using Shared.Domain.Events;

namespace Users.Domain.Aggregates.Profiles.Events;

public class ProfileCreatedDomainEvent : DomainEventBase
{
    public override string AggregateType => nameof(Profile);
    public override Guid AggregateId { get; }
    public override string Module => "users";

    public Guid UserId { get; }
    public string? Email { get; }
    public string? Name { get; }

    public ProfileCreatedDomainEvent(Guid aggregateId, Guid userId, string? email = null, string? name = null)
    {
        AggregateId = aggregateId;
        UserId = userId;
        Email = email;
        Name = name;
    }
}