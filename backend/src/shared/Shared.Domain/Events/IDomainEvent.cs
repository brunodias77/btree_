namespace Shared.Domain.Events;

/// <summary>
/// Evento com consequência dentro do próprio módulo.
/// Disparado e consumido dentro da mesma fronteira de contexto.
/// </summary>
public interface IDomainEvent : IEvent
{
}