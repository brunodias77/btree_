using BuildingBlocks.Domain.Events;

namespace Users.Domain.Aggregates.Session.Events;

/// <summary>
/// Domain event raised when a session is revoked.
/// </summary>
public sealed record SessionRevokedDomainEvent(
    Guid SessionId,
    Guid UserId,
    string? Reason) : DomainEvent;
