using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;
using Users.Domain.Aggregates.LoginHistory;
using Users.Domain.Repositories;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

public class LoginHistoryRepository : Repository<LoginHistory, Guid, UsersDbContext>, ILoginHistoryRepository
{
    public LoginHistoryRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LoginHistory>> GetByUserIdAsync(Guid userId, int limit = 10, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => EF.Property<DateTime>(x, "CreatedAt"))
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
