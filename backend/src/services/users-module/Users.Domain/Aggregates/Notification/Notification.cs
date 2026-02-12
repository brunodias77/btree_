using BuildingBlocks.Domain.Abstractions;
using Users.Domain.Aggregates.Notification.Events;

namespace Users.Domain.Aggregates.Notification;

/// <summary>
/// Notificação in-app para o usuário.
/// </summary>
public sealed class Notification : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }

    // Conteúdo
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string NotificationType { get; private set; } = null!;

    // Referência
    public string? ReferenceType { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public string? ActionUrl { get; private set; }

    // Controle
    public DateTime? ReadAt { get; private set; }
    // CreatedAt herdado de Entity<TId>

    public bool IsRead => ReadAt.HasValue;

    private Notification() : base() { }

    private Notification(
        Guid id,
        Guid userId,
        string title,
        string message,
        string notificationType) : base(id)
    {
        UserId = userId;
        Title = title;
        Message = message;
        NotificationType = notificationType;
        // CreatedAt inicializado na base
    }

    public static Notification Create(
        Guid userId,
        string title,
        string message,
        string notificationType,
        string? referenceType = null,
        Guid? referenceId = null,
        string? actionUrl = null)
    {
        return new Notification(Guid.NewGuid(), userId, title, message, notificationType)
        {
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            ActionUrl = actionUrl
        };
    }

    public void MarkAsRead()
    {
        if (ReadAt.HasValue)
            return;

        ReadAt = DateTime.UtcNow;

        RaiseDomainEvent(new NotificationReadDomainEvent(Id, UserId));
    }

    public void MarkAsUnread()
    {
        ReadAt = null;
    }
}
