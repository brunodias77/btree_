using Catalog.Domain.Aggregates.Product.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração da entidade ProductImage para o Entity Framework Core.
/// Mapeia a tabela catalog.product_images.
/// </summary>
public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property<Guid>("ProductId")
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(i => i.Url)
            .HasColumnName("url")
            .IsRequired();

        builder.Property(i => i.AltText)
            .HasColumnName("alt_text")
            .HasMaxLength(200);

        builder.Property(i => i.UrlThumbnail)
            .HasColumnName("thumbnail_url");

        builder.Property(i => i.UrlMedium)
            .HasColumnName("medium_url");

        builder.Property(i => i.UrlLarge)
            .HasColumnName("large_url");

        builder.Property(i => i.IsPrimary)
            .HasColumnName("is_primary")
            .HasDefaultValue(false);

        builder.Property(i => i.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0);

        // Audit fields
        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(i => i.DeletedAt)
            .HasColumnName("deleted_at");

        // Indexes
        builder.HasIndex("ProductId");
    }
}
