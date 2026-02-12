using Users.Application.Repositories;
using Users.Domain.Aggregates.Addresses;
using Users.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Infrastructure.Persistence;

namespace Users.Infrastructure.Data.Repositories;

public class AddressRepository : Repository<Address, Guid>, IAddressRepository
{
    public AddressRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Address?> GetDefaultAddressAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault, cancellationToken);
    }
}
