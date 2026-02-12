using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Events.Outbox;
/// <summary>
/// Mensagem de outbox para eventos de domínio.
/// Garante entrega confiável de eventos mesmo em caso de falhas.
/// Mapeado para tabela shared.domain_events do schema.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>
    /// Identificador único da mensagem.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Data/hora em que o evento ocorreu (UTC).
    /// Mapeado para 'occurred_at TIMESTAMPTZ'.
    /// </summary>
    public DateTime OccurredAt { get; set; }

    /// <summary>
    /// Tipo completo do evento (assembly qualified name).
    /// Mapeado para 'event_type VARCHAR(500)'.
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Payload serializado do evento (JSON).
    /// Mapeado para 'payload JSONB'.
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Data/hora em que a mensagem foi processada (UTC).
    /// Null se ainda não foi processada.
    /// Mapeado para 'processed_at TIMESTAMPTZ'.
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Mensagem de erro caso o processamento falhe.
    /// Mapeado para 'error TEXT'.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Número de tentativas de processamento.
    /// Mapeado para 'retry_count INT'.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Próxima tentativa de processamento (UTC).
    /// Mapeado para 'next_retry_at TIMESTAMPTZ'.
    /// </summary>
    public DateTime? NextRetryAt { get; set; }

    /// <summary>
    /// Número máximo de tentativas antes de desistir.
    /// </summary>
    public const int MaxRetryAttempts = 5;

    /// <summary>
    /// Indica se a mensagem foi processada.
    /// </summary>
    public bool IsProcessed => ProcessedAt.HasValue;

    /// <summary>
    /// Indica se excedeu o número máximo de tentativas.
    /// </summary>
    public bool HasExceededMaxRetries => RetryCount >= MaxRetryAttempts;

    /// <summary>
    /// Indica se a mensagem está pronta para processamento.
    /// </summary>
    public bool IsReadyForProcessing =>
        !IsProcessed &&
        !HasExceededMaxRetries &&
        (NextRetryAt is null || NextRetryAt <= DateTime.UtcNow);

    /// <summary>
    /// Marca a mensagem como processada com sucesso.
    /// </summary>
    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    /// <summary>
    /// Marca para retry com backoff exponencial.
    /// </summary>
    public void MarkForRetry(string error)
    {
        RetryCount++;
        Error = error;
        NextRetryAt = CalculateNextRetry();
    }

    /// <summary>
    /// Calcula próximo retry com backoff exponencial.
    /// Delays: 1min, 5min, 30min, 2h, 8h.
    /// </summary>
    private DateTime CalculateNextRetry()
    {
        var delays = new[] { 1, 5, 30, 120, 480 }; // minutos
        var delayMinutes = RetryCount <= delays.Length
            ? delays[RetryCount - 1]
            : delays[^1];

        return DateTime.UtcNow.AddMinutes(delayMinutes);
    }

    /// <summary>
    /// Marca como falha permanente.
    /// </summary>
    public void MarkAsFailed(string error)
    {
        ProcessedAt = DateTime.UtcNow;
        Error = $"[FALHA PERMANENTE] {error}";
        RetryCount = MaxRetryAttempts;
    }
}

/// <summary>
/// Configuração EF Core para OutboxMessage.
/// </summary>
public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("domain_events", "shared");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.OccurredAt)
            .IsRequired();

        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0);

        // Índice para busca de mensagens não processadas
        builder.HasIndex(x => new { x.ProcessedAt, x.NextRetryAt, x.RetryCount })
            .HasDatabaseName("ix_domain_events_pending");
    }
}
