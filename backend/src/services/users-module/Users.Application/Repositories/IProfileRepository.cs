using BuildingBlocks.Application.Data;
using Users.Domain.Aggregates.Profile;

namespace Users.Application.Repositories;

public interface IProfileRepository : IRepository<Profile, Guid>
{
    Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}