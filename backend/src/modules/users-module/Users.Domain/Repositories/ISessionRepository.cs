using Shared.Application.Data;
using Users.Domain.Aggregates.Sessions;

namespace Users.Domain.Repositories;

/// <summary>
/// Repositório para a entidade Session.
/// </summary>
public interface ISessionRepository : IRepository<Session, Guid>
{
    Task<Session?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
