using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Application.Events;

// <summary>
/// Interface para o barramento de eventos.
/// Responsável por publicar eventos de integração entre módulos.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publica um evento de integração.
    /// O evento será despachado para todos os handlers registrados.
    /// </summary>
    /// <typeparam name="TEvent">Tipo do evento.</typeparam>
    /// <param name="event">O evento a ser publicado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;

    /// <summary>
    /// Publica múltiplos eventos de integração.
    /// </summary>
    /// <param name="events">Eventos a serem publicados.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task PublishManyAsync(IEnumerable<IIntegrationEvent> events, CancellationToken cancellationToken = default);
}
