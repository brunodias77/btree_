using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Application.Repositories;
using Users.Domain.Aggregates.Session;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para a entidade Session.
/// </summary>
public class SessionRepository : Repository<Session, Guid>, ISessionRepository
{
    public SessionRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<Session?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == refreshTokenHash, cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await DbSet
            .Where(s => s.UserId == userId && !s.IsRevoked() && s.ExpiresAt > now)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Aqui buscamos todas as sessões ativas e as revogamos
        // Nota: Idealmente isso seria lógica de domínio ou serviço, mas o repositório auxilia na persistência em lote
        var activeSessions = await GetActiveSessionsAsync(userId, cancellationToken);
        
        foreach (var session in activeSessions)
        {
            session.Revoke(); // Método de domínio
            Update(session);
        }
    }
}
