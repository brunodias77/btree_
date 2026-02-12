using BuildingBlocks.Domain.Events;

namespace Users.Domain.Aggregates.Address.Events;

/// <summary>
/// Domain event raised when an address is set as default.
/// </summary>
public sealed record AddressSetAsDefaultDomainEvent(
    Guid AddressId,
    Guid UserId) : DomainEvent;
