using BuildingBlocks.Domain.Events;

namespace Users.Domain.Aggregates.Address.Events;

/// <summary>
/// Domain event raised when an address is updated.
/// </summary>
public sealed record AddressUpdatedDomainEvent(
    Guid AddressId,
    Guid UserId) : DomainEvent;
