using Shared.Domain.Events;

namespace Shared.Application.Events.Handlers;

// Handler estrito para eventos do próprio módulo
public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent> 
    where TEvent : IDomainEvent 
{ 
}