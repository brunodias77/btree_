using BuildingBlocks.Domain.Abstractions;
using Users.Domain.Aggregates.Session.Events;

namespace Users.Domain.Aggregates.Session;

/// <summary>
/// Sessão ativa do usuário (refresh token).
/// </summary>
public sealed class Session : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }

    // Token e dispositivo
    public string RefreshTokenHash { get; private set; } = null!;
    public string? DeviceId { get; private set; }
    public string? DeviceName { get; private set; }
    public string? DeviceType { get; private set; }

    // Localização
    public string? IpAddress { get; private set; }
    public string? Country { get; private set; }
    public string? City { get; private set; }

    // Controle
    public bool IsCurrent { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedReason { get; private set; }
    // CreatedAt herdado de Entity<TId>
    public DateTime LastActivityAt { get; private set; }

    private Session() : base() { }

    private Session(
        Guid id,
        Guid userId,
        string refreshTokenHash,
        DateTime expiresAt) : base(id)
    {
        UserId = userId;
        RefreshTokenHash = refreshTokenHash;
        ExpiresAt = expiresAt;
        // CreatedAt inicializado na base
        LastActivityAt = DateTime.UtcNow;
    }

    public static Session Create(
        Guid userId,
        string refreshTokenHash,
        DateTime expiresAt,
        string? deviceId = null,
        string? deviceName = null,
        string? deviceType = null,
        string? ipAddress = null,
        string? country = null,
        string? city = null,
        bool isCurrent = false)
    {
        var session = new Session(Guid.NewGuid(), userId, refreshTokenHash, expiresAt)
        {
            DeviceId = deviceId,
            DeviceName = deviceName,
            DeviceType = deviceType,
            IpAddress = ipAddress,
            Country = country,
            City = city,
            IsCurrent = isCurrent
        };

        session.RaiseDomainEvent(new SessionCreatedDomainEvent(session.Id, userId, deviceType, ipAddress));

        return session;
    }

    public void UpdateActivity(string? ipAddress = null)
    {
        LastActivityAt = DateTime.UtcNow;
        if (ipAddress != null)
            IpAddress = ipAddress;
    }

    public void SetAsCurrent()
    {
        IsCurrent = true;
        LastActivityAt = DateTime.UtcNow;
    }

    public void RemoveCurrent()
    {
        IsCurrent = false;
    }

    public void Revoke(string? reason = null)
    {
        if (RevokedAt.HasValue)
            return;

        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
        IsCurrent = false;

        RaiseDomainEvent(new SessionRevokedDomainEvent(Id, UserId, reason));
    }

    public bool IsValid()
    {
        return !RevokedAt.HasValue && ExpiresAt > DateTime.UtcNow;
    }

    public bool IsExpired()
    {
        return ExpiresAt <= DateTime.UtcNow;
    }

    public bool IsRevoked()
    {
        return RevokedAt.HasValue;
    }

    public void UpdateRefreshToken(string newRefreshTokenHash, DateTime newExpiresAt)
    {
        RefreshTokenHash = newRefreshTokenHash;
        ExpiresAt = newExpiresAt;
        LastActivityAt = DateTime.UtcNow;
    }
}
