using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Aggregates.LoginHistory;

/// <summary>
/// Histórico de login do usuário.
/// Registra tentativas de login (sucesso e falha).
/// </summary>
public sealed class LoginHistory : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }

    // Informações do login
    public string LoginProvider { get; private set; } = "Local";
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Country { get; private set; }
    public string? City { get; private set; }
    public string? DeviceType { get; private set; }
    public string? DeviceInfo { get; private set; } // JSON
    public bool Success { get; private set; }
    public string? FailureReason { get; private set; }

    // Controle
    // CreatedAt herdado de Entity<TId>

    private LoginHistory() : base() { }

    private LoginHistory(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
        // CreatedAt inicializado na base
    }

    public static LoginHistory CreateSuccess(
        Guid userId,
        string loginProvider = "Local",
        string? ipAddress = null,
        string? userAgent = null,
        string? country = null,
        string? city = null,
        string? deviceType = null,
        string? deviceInfo = null)
    {
        return new LoginHistory(Guid.NewGuid(), userId)
        {
            LoginProvider = loginProvider,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Country = country,
            City = city,
            DeviceType = deviceType,
            DeviceInfo = deviceInfo,
            Success = true
        };
    }

    public static LoginHistory CreateFailure(
        Guid userId,
        string failureReason,
        string loginProvider = "Local",
        string? ipAddress = null,
        string? userAgent = null,
        string? country = null,
        string? city = null,
        string? deviceType = null,
        string? deviceInfo = null)
    {
        return new LoginHistory(Guid.NewGuid(), userId)
        {
            LoginProvider = loginProvider,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Country = country,
            City = city,
            DeviceType = deviceType,
            DeviceInfo = deviceInfo,
            Success = false,
            FailureReason = failureReason
        };
    }
}
