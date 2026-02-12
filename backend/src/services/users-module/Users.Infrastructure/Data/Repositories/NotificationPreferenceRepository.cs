using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Application.Repositories;
using Users.Domain.Aggregates.NotificationPreference;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para a entidade NotificationPreference.
/// </summary>
public class NotificationPreferenceRepository : Repository<NotificationPreference, Guid>, INotificationPreferenceRepository
{
    public NotificationPreferenceRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(np => np.UserId == userId, cancellationToken);
    }
}
