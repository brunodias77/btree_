using BuildingBlocks.Application.Data;
using Users.Domain.Aggregates.NotificationPreference;

namespace Users.Application.Repositories;

public interface INotificationPreferenceRepository : IRepository<NotificationPreference, Guid>
{
    Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}