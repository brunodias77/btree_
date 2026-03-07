using Catalog.Domain.Aggregates.StockReservation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração da entidade StockReservation para o Entity Framework Core.
/// Mapeia a tabela catalog.stock_reservations.
/// </summary>
public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
{
    public void Configure(EntityTypeBuilder<StockReservation> builder)
    {
        builder.ToTable("stock_reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(r => r.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(r => r.ReferenceType)
            .HasColumnName("reference_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.ReferenceId)
            .HasColumnName("reference_id")
            .IsRequired();

        builder.Property(r => r.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(r => r.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(r => r.ReleasedAt)
            .HasColumnName("released_at");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Unique constraint
        builder.HasIndex(r => new { r.ProductId, r.ReferenceType, r.ReferenceId })
            .IsUnique();

        // Indexes
        builder.HasIndex(r => r.ProductId);

        builder.HasIndex(r => r.ExpiresAt);

        builder.HasIndex(r => new { r.ReferenceType, r.ReferenceId });
    }
}
