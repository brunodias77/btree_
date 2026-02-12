using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Application.Repositories;
using Users.Domain.Aggregates.Notification;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para a entidade Notification.
/// </summary>
public class NotificationRepository : Repository<Notification, Guid>, INotificationRepository
{
    public NotificationRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }
}
