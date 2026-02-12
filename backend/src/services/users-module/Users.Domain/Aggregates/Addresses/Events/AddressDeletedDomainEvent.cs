using BuildingBlocks.Domain.Events;

namespace Users.Domain.Aggregates.Address.Events;

/// <summary>
/// Domain event raised when an address is deleted.
/// </summary>
public sealed record AddressDeletedDomainEvent(
    Guid AddressId,
    Guid UserId) : DomainEvent;
