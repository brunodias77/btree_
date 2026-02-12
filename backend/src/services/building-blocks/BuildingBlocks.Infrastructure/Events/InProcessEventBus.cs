using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Events;

/// <summary>
/// Implementação do event bus in-process usando MediatR.
/// Publica eventos de integração entre módulos no mesmo processo.
/// </summary>
public sealed class InProcessEventBus : IEventBus
{
    private readonly IPublisher _publisher;
    private readonly ILogger<InProcessEventBus> _logger;

    public InProcessEventBus(IPublisher publisher, ILogger<InProcessEventBus> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    /// <summary>
    /// Publica um evento de integração.
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        _logger.LogDebug(
            "Publicando evento de integração {EventType} com ID {EventId}",
            @event.EventType,
            @event.EventId);

        try
        {
            await _publisher.Publish(@event, cancellationToken);

            _logger.LogInformation(
                "Evento de integração {EventType} publicado com sucesso",
                @event.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao publicar evento de integração {EventType}",
                @event.EventType);
            throw;
        }
    }

    /// <summary>
    /// Publica múltiplos eventos de integração.
    /// </summary>
    public async Task PublishManyAsync(
        IEnumerable<IIntegrationEvent> events,
        CancellationToken cancellationToken = default)
    {
        var eventList = events.ToList();

        _logger.LogDebug(
            "Publicando {Count} eventos de integração",
            eventList.Count);

        foreach (var @event in eventList)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }
}
