using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;
using Users.Domain.Aggregates.Addresses;
using Users.Domain.Repositories;
using Users.Infrastructure.Data.Context;

namespace Users.Infrastructure.Data.Repositories;

public class AddressRepository : Repository<Address, Guid, UsersDbContext>, IAddressRepository
{
    public AddressRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Address?> GetDefaultAddressAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsDefault, cancellationToken);
    }
}
