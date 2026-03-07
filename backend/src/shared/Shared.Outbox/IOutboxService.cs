using Shared.Domain.Events;

namespace Shared.Outbox;

public interface IOutboxService
{
    // Método que os seus módulos usarão em vez de chamar o EventDispatcher diretamente na request HTTP original
    Task SaveAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
}
