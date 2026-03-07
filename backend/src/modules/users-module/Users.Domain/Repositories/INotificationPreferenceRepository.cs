using Shared.Application.Data;
using Users.Domain.Aggregates.NotificationsPreference;

namespace Users.Domain.Repositories;

public interface INotificationPreferenceRepository : IRepository<NotificationPreference, Guid>
{
    Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}