using BuildingBlocks.Application.Data;
using Users.Domain.Aggregates.Session;

namespace Users.Application.Repositories;
/// <summary>
/// Repositório para a entidade Session.
/// </summary>
public interface ISessionRepository : IRepository<Session, Guid>
{
    Task<Session?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
