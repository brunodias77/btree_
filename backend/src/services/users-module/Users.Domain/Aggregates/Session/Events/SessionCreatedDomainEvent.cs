using BuildingBlocks.Domain.Events;

namespace Users.Domain.Aggregates.Session.Events;

/// <summary>
/// Domain event raised when a new session is created.
/// </summary>
public sealed record SessionCreatedDomainEvent(
    Guid SessionId,
    Guid UserId,
    string? DeviceType,
    string? IpAddress) : DomainEvent;
