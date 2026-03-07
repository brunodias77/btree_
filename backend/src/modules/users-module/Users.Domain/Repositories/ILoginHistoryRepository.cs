using Shared.Application.Data;
using Users.Domain.Aggregates.LoginHistory;

namespace Users.Domain.Repositories;

public interface ILoginHistoryRepository : IRepository<LoginHistory, Guid>
{
    Task<IEnumerable<LoginHistory>> GetByUserIdAsync(Guid userId, int limit = 10, CancellationToken cancellationToken = default);
}
