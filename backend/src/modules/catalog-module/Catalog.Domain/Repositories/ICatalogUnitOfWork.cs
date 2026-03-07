using Shared.Application.Data;

namespace Catalog.Domain.Repositories;

/// <summary>
/// Interface marcadora para o Unit of Work do Catálogo.
/// Herda de IUnitOfWork para manter compatibilidade de métodos, 
/// mas permite injeção específica para evitar colisão com outros módulos.
/// </summary>
public interface ICatalogUnitOfWork : IUnitOfWork
{
}
