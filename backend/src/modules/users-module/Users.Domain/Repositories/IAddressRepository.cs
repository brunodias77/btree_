using Shared.Application.Data;
using Users.Domain.Aggregates.Addresses;

namespace Users.Domain.Repositories;

public interface IAddressRepository : IRepository<Address, Guid>
{
    Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Address?> GetDefaultAddressAsync(Guid userId, CancellationToken cancellationToken = default);
}