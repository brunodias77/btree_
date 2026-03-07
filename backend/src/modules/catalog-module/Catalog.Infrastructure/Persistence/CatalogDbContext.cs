using System.Reflection;
using Shared.Infrastructure.Data;
using Catalog.Domain.Aggregates.Brand;
using Catalog.Domain.Aggregates.Category;
using Catalog.Domain.Aggregates.Product;
using Catalog.Domain.Aggregates.Product.Entities;
using Catalog.Domain.Aggregates.ProductReview;
using Catalog.Domain.Aggregates.StockMovement;
using Catalog.Domain.Aggregates.StockReservation;
using Catalog.Domain.Aggregates.UserFavorite;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

/// <summary>
/// Contexto de banco de dados do módulo de Catálogo.
/// OutboxMessages e InboxMessages são herdados de <see cref="BaseDbContext"/>
/// e mapeados automaticamente para shared.domain_events / shared.processed_events.
/// </summary>
public sealed class CatalogDbContext : BaseDbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options) { }

    // ── Entidades do módulo ───────────────────────────────────────────────────

    /// <summary>Categorias do catálogo.</summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>Marcas de produtos.</summary>
    public DbSet<Brand> Brands => Set<Brand>();

    /// <summary>Produtos do catálogo.</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>Imagens de produtos.</summary>
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    /// <summary>Movimentações de estoque.</summary>
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    /// <summary>Reservas de estoque.</summary>
    public DbSet<StockReservation> StockReservations => Set<StockReservation>();

    /// <summary>Avaliações de produtos.</summary>
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();

    /// <summary>Produtos favoritos dos usuários.</summary>
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();

    // ─────────────────────────────────────────────────────────────────────────

    protected override string Schema => "catalog";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // BaseDbContext já chama ApplyBaseModelConfigurations(Schema, GetType().Assembly),
        // que aplica as configs de Outbox/Inbox (shared) + as configs deste assembly (catalog).
        base.OnModelCreating(modelBuilder);
    }
}