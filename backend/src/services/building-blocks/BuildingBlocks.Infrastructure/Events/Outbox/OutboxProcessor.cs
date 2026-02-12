using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Events.Outbox;

/// <summary>
/// Processador de mensagens do outbox.
/// Lê mensagens pendentes e as processa usando MediatR.
/// Implementa retry com backoff exponencial.
/// </summary>
public sealed class OutboxProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Processa mensagens pendentes do outbox.
    /// </summary>
    /// <param name="batchSize">Número máximo de mensagens a processar por vez.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    public async Task ProcessAsync(int batchSize = 20, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();

        var now = DateTime.UtcNow;

        // Obtém mensagens prontas para processamento
        var messages = await context.Set<OutboxMessage>()
            .Where(m =>
                m.ProcessedAt == null &&
                m.RetryCount < OutboxMessage.MaxRetryAttempts &&
                (m.NextRetryAt == null || m.NextRetryAt <= now))
            .OrderBy(m => m.OccurredAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            _logger.LogDebug("Nenhuma mensagem pendente no outbox");
            return;
        }

        _logger.LogInformation("Processando {Count} mensagens do outbox", messages.Count);

        foreach (var message in messages)
        {
            await ProcessMessageAsync(scope.ServiceProvider, context, message, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Processa uma mensagem individual.
    /// </summary>
    private async Task ProcessMessageAsync(
        IServiceProvider serviceProvider,
        DbContext context,
        OutboxMessage message,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug(
                "Processando mensagem {MessageId} do tipo {EventType}",
                message.Id,
                message.EventType);

            // Resolve o tipo do evento
            var eventType = Type.GetType(message.EventType);
            if (eventType is null)
            {
                message.MarkAsFailed($"Tipo de evento não encontrado: {message.EventType}");
                _logger.LogError("Tipo de evento não encontrado: {EventType}", message.EventType);
                return;
            }

            // Deserializa o evento
            var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType);
            if (domainEvent is null)
            {
                message.MarkAsFailed("Falha ao deserializar o evento");
                _logger.LogError("Falha ao deserializar evento {MessageId}", message.Id);
                return;
            }

            // Publica usando MediatR
            var publisher = serviceProvider.GetRequiredService<MediatR.IPublisher>();
            await publisher.Publish(domainEvent, cancellationToken);

            // Marca como processado
            message.MarkAsProcessed();

            _logger.LogDebug(
                "Mensagem {MessageId} processada com sucesso",
                message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao processar mensagem {MessageId}: {Error}",
                message.Id,
                ex.Message);

            message.MarkForRetry(ex.Message);

            if (message.HasExceededMaxRetries)
            {
                _logger.LogWarning(
                    "Mensagem {MessageId} excedeu número máximo de tentativas",
                    message.Id);
            }
        }
    }
}
