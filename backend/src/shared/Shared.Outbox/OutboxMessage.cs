namespace Shared.Outbox;

/// <summary>
/// Representa um evento persistido na tabela shared.domain_events (Outbox Pattern).
/// Será processado por um worker em background de forma garantida.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    // Módulo de origem (ex: "orders", "catalog", "payments")
    public string Module { get; init; } = string.Empty;
    
    // Tipo do agregado (ex: "Order", "Product")
    public string AggregateType { get; init; } = string.Empty;
    
    // Id do agregado que originou o evento
    public Guid AggregateId { get; init; }
    
    // Nome completo do tipo para desserialização (AssemblyQualifiedName)
    public string EventType { get; init; } = string.Empty;
    
    // Conteúdo serializado em JSON do evento original
    public string Payload { get; init; } = string.Empty;
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public DateTime? ProcessedAt { get; private set; }
    
    public string? ErrorMessage { get; private set; }
    
    public int RetryCount { get; private set; } = 0;

    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void MarkAsFailed(string error)
    {
        ErrorMessage = error;
        RetryCount++;
    }
}
