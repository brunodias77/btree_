using BuildingBlocks.Application.Data;
using Users.Domain.Aggregates.Addresses;

namespace Users.Application.Repositories;

/// <summary>
/// Repositório para a entidade Address.
/// </summary>
public interface IAddressRepository : IRepository<Address, Guid>
{
    Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Address?> GetDefaultAddressAsync(Guid userId, CancellationToken cancellationToken = default);
}