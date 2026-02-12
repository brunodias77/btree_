using MediatR;

namespace BuildingBlocks.Domain.Events;

/// <summary>
/// Interface marcadora para eventos de integração.
/// Eventos de integração são usados para comunicação entre módulos/bounded contexts.
/// Diferente de eventos de domínio, são publicados após commit da transação.
/// </summary>
public interface IIntegrationEvent : INotification
{
    /// <summary>
    /// Identificador único do evento.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Data/hora em que o evento ocorreu (UTC).
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Tipo do evento (nome completo para serialização).
    /// </summary>
    string EventType { get; }
}

/// <summary>
/// Implementação base para eventos de integração.
/// </summary>
public abstract record IntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Identificador único do evento.
    /// </summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Data/hora em que o evento ocorreu (UTC).
    /// </summary>
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Tipo do evento (nome completo para serialização).
    /// </summary>
    public string EventType => GetType().FullName ?? GetType().Name;
}
