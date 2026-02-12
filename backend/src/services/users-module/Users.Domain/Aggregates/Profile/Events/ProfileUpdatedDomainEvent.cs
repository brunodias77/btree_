using BuildingBlocks.Domain.Events;

namespace Users.Domain.Aggregates.Profile.Events;

/// <summary>
/// Domain event raised when a profile is updated.
/// </summary>
public sealed record ProfileUpdatedDomainEvent(
    Guid ProfileId,
    Guid UserId) : DomainEvent;
