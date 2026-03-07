using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Application.Events.Handlers;
using Shared.Domain.Events;

namespace Shared.Outbox;

public class ProcessOutboxMessagesBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProcessOutboxMessagesBackgroundService> _logger;
    
    private const int BatchSize = 20;
    private const int MaxRetries = 5;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(10);

    public ProcessOutboxMessagesBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ProcessOutboxMessagesBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro fatal no loop de processamento do Outbox.");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }
        
        _logger.LogInformation("Outbox processor finalizado.");
    }

    private async Task ProcessMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var inboxRepository = scope.ServiceProvider.GetRequiredService<IInboxRepository>();
        var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        _logger.LogInformation("Verificando mensagens pendentes no Outbox...");
        var messages = await outboxRepository.GetUnprocessedMessagesAsync(BatchSize, MaxRetries, stoppingToken);
        
        if (messages.Count > 0)
        {
            _logger.LogInformation("Processando {Count} mensagem(ns) do Outbox (BatchSize: {BatchSize})", messages.Count, BatchSize);
        }
        else
        {
            _logger.LogInformation("Nenhuma mensagem não processada encontrada no Outbox.");
        }

        foreach (var message in messages)
        {
            try
            {
                // 1. Verificar idempotência via Inbox
                if (await inboxRepository.WasProcessedAsync(message.Id, stoppingToken))
                {
                    _logger.LogWarning("Evento {EventId} já foi processado (inbox). Marcando como concluído.", message.Id);
                    message.MarkAsProcessed();
                    await outboxRepository.UpdateAsync(message, stoppingToken);
                    continue;
                }

                // 2. Resolver o tipo do evento
                var eventType = Type.GetType(message.EventType);
                if (eventType == null)
                {
                    _logger.LogError("Tipo de evento não encontrado: {EventType}", message.EventType);
                    message.MarkAsFailed($"Tipo de evento não encontrado: {message.EventType}");
                    await outboxRepository.UpdateAsync(message, stoppingToken);
                    continue;
                }

                // 3. Desserializar o evento
                var @event = JsonSerializer.Deserialize(message.Payload, eventType) as IEvent;
                if (@event == null)
                {
                    _logger.LogError("Falha na desserialização do evento {EventId} ({EventType})", message.Id, message.EventType);
                    message.MarkAsFailed("Falha na desserialização do evento.");
                    await outboxRepository.UpdateAsync(message, stoppingToken);
                    continue;
                }

                // 4. Disparar o evento via Reflection (IEventDispatcher.PublishAsync é genérico)
                var method = typeof(IEventDispatcher).GetMethod(nameof(IEventDispatcher.PublishAsync));
                if (method != null)
                {
                    var genericMethod = method.MakeGenericMethod(eventType);
                    var task = (Task)genericMethod.Invoke(eventDispatcher, new object[] { @event, stoppingToken })!;
                    await task;
                }

                // 5. Marcar como processado no Outbox e no Inbox
                message.MarkAsProcessed();
                await outboxRepository.UpdateAsync(message, stoppingToken);
                await inboxRepository.MarkAsProcessedAsync(message.Id, message.EventType, message.Module, stoppingToken);
                
                _logger.LogDebug("Evento {EventId} ({EventType}) processado com sucesso.", message.Id, message.EventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao processar mensagem do Outbox {EventId} (tentativa {RetryCount})", 
                    message.Id, message.RetryCount + 1);
                message.MarkAsFailed(ex.InnerException?.Message ?? ex.Message);
                await outboxRepository.UpdateAsync(message, stoppingToken);
            }
        }
    }
}
