using Shared.Application.Data;
using Users.Domain.Aggregates.Notifications;

namespace Users.Domain.Repositories;

public interface INotificationRepository : IRepository<Notification, Guid>
{
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}