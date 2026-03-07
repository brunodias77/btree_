using Shared.Application.Data;
using Users.Domain.Aggregates.Profiles;

namespace Users.Domain.Repositories;

public interface IProfileRepository : IRepository<Profile, Guid>
{
    Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Profile?> GetByPasswordResetCodeAsync(string code, CancellationToken cancellationToken = default);
}