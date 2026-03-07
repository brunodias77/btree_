using Microsoft.EntityFrameworkCore;
using Shared.Application.Data;

namespace Shared.Infrastructure.Data;

/// <summary>
/// Implementação base de Unit of Work usando EF Core.
/// Cada módulo deve herdar desta classe com seu DbContext específico.
/// </summary>
/// <typeparam name="TContext">Tipo do DbContext.</typeparam>
public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}