using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;
using Users.Domain.Aggregates.Sessions;
using Users.Domain.Repositories;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

public class SessionRepository : Repository<Session, Guid, UsersDbContext>, ISessionRepository
{
    public SessionRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<Session?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(x => x.RefreshTokenHash == refreshTokenHash, cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = System.DateTime.UtcNow;
        return await DbSet
            .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > now)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = System.DateTime.UtcNow;
        var activeSessions = await DbSet
            .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > now)
            .ToListAsync(cancellationToken);

        foreach (var session in activeSessions)
        {
            session.Revoke("Revoked globally by user action");
        }
        
        DbSet.UpdateRange(activeSessions);
    }
}
