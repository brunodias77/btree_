using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Aggregates.NotificationPreference;

/// <summary>
/// Preferências de notificação do usuário.
/// </summary>
public sealed class NotificationPreference : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }

    // Canais de notificação
    public bool EmailEnabled { get; private set; } = true;
    public bool PushEnabled { get; private set; } = true;
    public bool SmsEnabled { get; private set; }

    // Tipos de notificação
    public bool OrderUpdates { get; private set; } = true;
    public bool Promotions { get; private set; } = true;
    public bool PriceDrops { get; private set; } = true;
    public bool BackInStock { get; private set; } = true;
    public bool Newsletter { get; private set; }

    // Controle
    // CreatedAt, UpdatedAt herdados de Entity<TId>

    private NotificationPreference() : base() { }

    private NotificationPreference(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
        // CreatedAt inicializado na base
    }

    public static NotificationPreference CreateDefault(Guid userId)
    {
        return new NotificationPreference(Guid.NewGuid(), userId);
    }

    public void Update(
        bool? emailEnabled,
        bool? pushEnabled,
        bool? smsEnabled,
        bool? orderUpdates,
        bool? promotions,
        bool? priceDrops,
        bool? backInStock,
        bool? newsletter)
    {
        EmailEnabled = emailEnabled ?? EmailEnabled;
        PushEnabled = pushEnabled ?? PushEnabled;
        SmsEnabled = smsEnabled ?? SmsEnabled;
        OrderUpdates = orderUpdates ?? OrderUpdates;
        Promotions = promotions ?? Promotions;
        PriceDrops = priceDrops ?? PriceDrops;
        BackInStock = backInStock ?? BackInStock;
        Newsletter = newsletter ?? Newsletter;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableAllChannels()
    {
        EmailEnabled = true;
        PushEnabled = true;
        SmsEnabled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DisableAllChannels()
    {
        EmailEnabled = false;
        PushEnabled = false;
        SmsEnabled = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableAllNotifications()
    {
        OrderUpdates = true;
        Promotions = true;
        PriceDrops = true;
        BackInStock = true;
        Newsletter = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DisableAllNotifications()
    {
        OrderUpdates = false;
        Promotions = false;
        PriceDrops = false;
        BackInStock = false;
        Newsletter = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanReceiveEmail() => EmailEnabled;
    public bool CanReceivePush() => PushEnabled;
    public bool CanReceiveSms() => SmsEnabled;
}
