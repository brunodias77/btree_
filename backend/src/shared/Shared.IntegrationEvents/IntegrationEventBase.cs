using Shared.Application.Events;

namespace Shared.IntegrationEvents;


/// <summary>
/// Classe base para todos os integration events.
/// Garante que EventId, OccurredOn e Module sejam sempre preenchidos corretamente.
/// </summary>
public abstract class IntegrationEventBase : IIntegrationEvent
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
