using MediatR;

namespace BuildingBlocks.Domain.Events;
/// <summary>
/// Interface marcadora para eventos de domínio.
/// Eventos de domínio representam algo que aconteceu no domínio e são usados para
/// comunicar mudanças entre agregados e módulos.
/// Implementa INotification do MediatR para despacho in-process.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Identificador único do evento.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Data/hora em que o evento ocorreu (UTC).
    /// </summary>
    DateTime OccurredAt { get; }
}

/// <summary>
/// Implementação base para eventos de domínio.
/// Fornece valores padrão para EventId e OccurredAt.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// Identificador único do evento.
    /// </summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Data/hora em que o evento ocorreu (UTC).
    /// </summary>
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
