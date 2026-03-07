namespace Shared.Application.Events.Handlers;

// Handler estrito para eventos de outros módulos
public interface IIntegrationEventHandler<in TEvent> : IEventHandler<TEvent> 
    where TEvent : IIntegrationEvent 
{ 
}