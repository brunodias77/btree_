namespace Shared.Outbox;

public interface IOutboxRepository
{
    // Adiciona uma nova mensagem no Outbox (deve rodar na mesma Unit of Work / DbContext da operação principal)
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    
    // Processamento do Worker: busca mensagens não processadas que não excederam o limite de retentativas
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 20, int maxRetries = 5, CancellationToken cancellationToken = default);
    
    // Processamento do Worker: atualiza o estado de erro/sucesso após tentar disparar
    Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default);
}
