using System.Text.Json;
using Shared.Domain.Events;

namespace Shared.Outbox;

public class OutboxService : IOutboxService
{
    private readonly IOutboxRepository _outboxRepository;

    public OutboxService(IOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }

    public async Task SaveAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        // Deriva o módulo a partir do namespace do evento (ex: "Orders.Application.Events" -> "orders")
        var eventTypeName = @event.GetType().Namespace ?? string.Empty;
        var module = eventTypeName.Split('.').FirstOrDefault()?.ToLowerInvariant() ?? "unknown";

        var outboxMessage = new OutboxMessage
        {
            Module = module,
            AggregateType = @event.AggregateType,
            AggregateId = @event.AggregateId,
            EventType = @event.GetType().FullName ?? @event.GetType().Name,
            // Serializa usando o tipo concreto para não perder propriedades
            Payload = JsonSerializer.Serialize(@event, @event.GetType())
        };

        await _outboxRepository.AddAsync(outboxMessage, cancellationToken);
    }
}
