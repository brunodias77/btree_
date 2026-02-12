using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Application.Repositories;
using Users.Domain.Aggregates.LoginHistory;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para a entidade LoginHistory.
/// </summary>
public class LoginHistoryRepository : Repository<LoginHistory, Guid>, ILoginHistoryRepository
{
    public LoginHistoryRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LoginHistory>> GetByUserIdAsync(Guid userId, int limit = 10, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(lh => lh.UserId == userId)
            .OrderByDescending(lh => lh.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
