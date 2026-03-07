namespace Shared.Outbox;
/// <summary>
/// Entidade compartilhada para o Inbox pattern (idempotência).
/// Mapeia a tabela shared.processed_events.
/// Centralizada no Shared.Infrastructure para evitar duplicação nos módulos.
/// </summary>
public sealed class InboxMessage
{
    public Guid Id { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string Module { get; init; } = string.Empty;
    public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;
}