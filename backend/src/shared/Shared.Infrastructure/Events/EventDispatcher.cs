using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Application.Events.Handlers;
using Shared.Domain.Events;

namespace Shared.Infrastructure.Events;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventDispatcher> _logger;

    public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IEvent
    {
        // Para os outros, buscamos os handlers no contêiner de injeção de dependência
        var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();

        var exceptions = new List<Exception>();

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao executar handler {HandlerType} para evento {EventType}", 
                    handler.GetType().Name, @event.GetType().Name);
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException(
                $"Falha ao processar {exceptions.Count} handler(s) do evento {@event.GetType().Name}", 
                exceptions);
        }
    }
}