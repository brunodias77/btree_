namespace Shared.Domain.Events;

/// <summary>
/// Classe base para todos os domain events.
/// Garante que EventId e OccurredOn sejam sempre preenchidos corretamente.
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <inheritdoc/>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <inheritdoc/>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc/>
    public abstract string AggregateType { get; }

    /// <inheritdoc/>
    public abstract Guid AggregateId { get; }

    /// <inheritdoc/>
    public abstract string Module { get; }
}