using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Infrastructure.Events.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Events;

/// <summary>
/// EventBus que persiste eventos no Outbox em vez de publicá-los imediatamente.
/// Garante consistência transacional.
/// </summary>
public sealed class OutboxEventBus : IEventBus
{
    private readonly DbContext _dbContext;
    private readonly ILogger<OutboxEventBus> _logger;

    public OutboxEventBus(DbContext dbContext, ILogger<OutboxEventBus> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        var outboxMessage = CreateOutboxMessage(@event);
        
        _dbContext.Set<OutboxMessage>().Add(outboxMessage);
        
        _logger.LogDebug("Evento de integração {EventType} adicionado ao Outbox", outboxMessage.EventType);
        
        return Task.CompletedTask;
    }

    public Task PublishManyAsync(IEnumerable<IIntegrationEvent> events, CancellationToken cancellationToken = default)
    {
        var messages = events.Select(CreateOutboxMessage).ToList();
        
        if (messages.Any())
        {
            _dbContext.Set<OutboxMessage>().AddRange(messages);
            _logger.LogDebug("{Count} eventos de integração adicionados ao Outbox", messages.Count);
        }

        return Task.CompletedTask;
    }

    private static OutboxMessage CreateOutboxMessage(IIntegrationEvent @event)
    {
        var eventType = @event.GetType();
        
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow, // Poderia usar @event.OccurredAt se disponível na interface
            EventType = eventType.AssemblyQualifiedName ?? eventType.FullName ?? eventType.Name,
            Payload = JsonSerializer.Serialize(@event, eventType, new JsonSerializerOptions
            {
                WriteIndented = false
            })
        };
    }
}
