using BuildingBlocks.Domain.Events;

namespace Users.Domain.Aggregates.Profile.Events;

/// <summary>
/// Domain event raised when a new profile is created.
/// </summary>
public sealed record ProfileCreatedDomainEvent(
    Guid ProfileId,
    Guid UserId,
    string Email,
    string Name) : DomainEvent;
