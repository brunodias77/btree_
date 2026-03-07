using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;
using Users.Domain.Aggregates.Notifications;
using Users.Domain.Repositories;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

public class NotificationRepository : Repository<Notification, Guid, UsersDbContext>, INotificationRepository
{
    public NotificationRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.UserId == userId && x.ReadAt == null)
            .OrderByDescending(x => EF.Property<DateTime>(x, "CreatedAt"))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(x => x.UserId == userId && x.ReadAt == null, cancellationToken);
    }
}
