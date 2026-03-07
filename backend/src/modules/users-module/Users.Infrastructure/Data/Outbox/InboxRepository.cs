using Microsoft.EntityFrameworkCore;
using Shared.Outbox;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Outbox;

/// <summary>
/// Implementação do Inbox repository usando o <see cref="InboxMessage"/> compartilhado
/// de Shared.Infrastructure (shared.processed_events).
/// </summary>
public class InboxRepository : IInboxRepository
{
    private readonly UsersDbContext _dbContext;

    public InboxRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> WasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InboxMessages
            .AnyAsync(m => m.Id == eventId, cancellationToken);
    }

    public async Task MarkAsProcessedAsync(
        Guid eventId,
        string eventType,
        string module,
        CancellationToken cancellationToken = default)
    {
        var inboxMessage = new InboxMessage
        {
            Id = eventId,
            EventType = eventType,
            Module = module,
            ProcessedAt = DateTime.UtcNow
        };

        await _dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}