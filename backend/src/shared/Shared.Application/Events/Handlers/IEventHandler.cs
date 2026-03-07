using Shared.Domain.Events;

namespace Shared.Application.Events.Handlers;

// Contrato base para processar eventos
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}