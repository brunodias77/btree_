using BuildingBlocks.Domain.Events;

namespace Users.Domain.Aggregates.Address.Events;

/// <summary>
/// Domain event raised when a new address is created.
/// </summary>
public sealed record AddressCreatedDomainEvent(
    Guid AddressId,
    Guid UserId) : DomainEvent;
