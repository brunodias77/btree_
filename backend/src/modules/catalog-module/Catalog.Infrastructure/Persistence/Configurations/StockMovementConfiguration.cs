using Catalog.Domain.Aggregates.StockMovement;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração da entidade StockMovement para o Entity Framework Core.
/// Mapeia a tabela catalog.stock_movements.
/// </summary>
public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("stock_movements");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(m => m.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(m => m.MovementType)
            .HasColumnName("movement_type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(m => m.ReferenceType)
            .HasColumnName("reference_type")
            .HasMaxLength(50);

        builder.Property(m => m.ReferenceId)
            .HasColumnName("reference_id");

        builder.Property(m => m.StockBefore)
            .HasColumnName("stock_before")
            .IsRequired();

        builder.Property(m => m.StockAfter)
            .HasColumnName("stock_after")
            .IsRequired();

        builder.Property(m => m.Reason)
            .HasColumnName("reason");

        builder.Property(m => m.PerformedBy)
            .HasColumnName("performed_by");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(m => m.ProductId);

        builder.HasIndex(m => m.CreatedAt);

        builder.HasIndex(m => new { m.ReferenceType, m.ReferenceId });
    }
}
