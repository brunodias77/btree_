namespace BuildingBlocks.Infrastructure.Events.Outbox;
/// <summary>
/// Configurações para o processador de outbox.
/// </summary>
public sealed class OutboxConfiguration
{
    /// <summary>
    /// Nome da seção de configuração.
    /// </summary>
    public const string SectionName = "Outbox";

    /// <summary>
    /// Intervalo de polling para buscar novas mensagens.
    /// Padrão: 10 segundos.
    /// </summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Número máximo de mensagens a processar por batch.
    /// Padrão: 20 mensagens.
    /// </summary>
    public int BatchSize { get; set; } = 20;

    /// <summary>
    /// Número máximo de tentativas antes de desistir.
    /// Padrão: 5 tentativas.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 5;

    /// <summary>
    /// Período de retenção para mensagens processadas.
    /// Mensagens mais antigas serão limpas pelo job de cleanup.
    /// Padrão: 7 dias.
    /// </summary>
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    /// Indica se o processador de outbox está habilitado.
    /// Padrão: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Indica se deve processar mensagens na ordem de criação.
    /// Desabilitar pode melhorar performance mas perde ordenação.
    /// Padrão: true.
    /// </summary>
    public bool ProcessInOrder { get; set; } = true;
}
