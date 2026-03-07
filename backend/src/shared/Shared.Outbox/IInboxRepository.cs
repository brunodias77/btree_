namespace Shared.Outbox;

/// <summary>
/// Inbox pattern: garante idempotência no processamento de eventos.
/// Corresponde à tabela shared.processed_events no SQL.
/// </summary>
public interface IInboxRepository
{
    // Verifica se um evento já foi processado (idempotência)
    Task<bool> WasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);
    
    // Registra que um evento foi processado com sucesso
    Task MarkAsProcessedAsync(Guid eventId, string eventType, string module, CancellationToken cancellationToken = default);
}
