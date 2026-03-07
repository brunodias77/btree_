using Microsoft.EntityFrameworkCore;
using Shared.Outbox;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Outbox;

public class OutboxRepository : IOutboxRepository
{
    private readonly UsersDbContext _dbContext;

    public OutboxRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<OutboxMessage>().AddAsync(message, cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 20, int maxRetries = 5, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedAt == null && m.RetryCount < maxRetries)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<OutboxMessage>().Update(message);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
