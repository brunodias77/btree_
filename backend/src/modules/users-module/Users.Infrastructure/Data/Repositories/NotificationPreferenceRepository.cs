using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;
using Users.Domain.Aggregates.NotificationsPreference;
using Users.Domain.Repositories;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

public class NotificationPreferenceRepository : Repository<NotificationPreference, Guid, UsersDbContext>, INotificationPreferenceRepository
{
    public NotificationPreferenceRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}
