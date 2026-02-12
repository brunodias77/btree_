using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Application.Repositories;
using Users.Domain.Aggregates.Profile;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para a entidade Profile.
/// </summary>
public class ProfileRepository : Repository<Profile, Guid>, IProfileRepository
{
    public ProfileRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }
}
