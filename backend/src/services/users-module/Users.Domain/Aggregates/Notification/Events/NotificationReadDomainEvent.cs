using BuildingBlocks.Domain.Events;

namespace Users.Domain.Aggregates.Notification.Events;

/// <summary>
/// Domain event raised when a notification is read.
/// </summary>
public sealed record NotificationReadDomainEvent(
    Guid NotificationId,
    Guid UserId) : DomainEvent;
