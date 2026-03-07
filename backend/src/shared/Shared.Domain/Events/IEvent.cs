namespace Shared.Domain.Events;


/// <summary>
/// Interface base para todos os eventos do sistema.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Identificador único do evento.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Data/hora em que o evento ocorreu (UTC).
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Tipo do agregado que originou o evento (ex: "Order", "User").
    /// </summary>
    string AggregateType { get; }

    /// <summary>
    /// ID do agregado que originou o evento.
    /// </summary>
    Guid AggregateId { get; }

    /// <summary>
    /// Módulo de origem do evento (ex: "orders", "catalog", "users").
    /// Usado pelo Outbox para roteamento e rastreamento.
    /// </summary>
    string Module { get; }
}
