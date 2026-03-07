using Catalog.Domain.Aggregates.Product;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração da entidade Product para o Entity Framework Core.
/// Mapeia a tabela catalog.products.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Foreign Keys
        builder.Property(p => p.CategoryId)
            .HasColumnName("category_id");

        builder.Property(p => p.BrandId)
            .HasColumnName("brand_id");

        // Identification
        builder.Property(p => p.Sku)
            .HasColumnName("sku")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Barcode)
            .HasColumnName("barcode")
            .HasMaxLength(50);

        // Description
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.ShortDescription)
            .HasColumnName("short_description")
            .HasMaxLength(500);

        builder.Property(p => p.Description)
            .HasColumnName("full_description");

        // Pricing
        builder.Property(p => p.Price)
            .HasColumnName("price")
            .HasPrecision(15, 2)
            .IsRequired();

        builder.Property(p => p.CompareAtPrice)
            .HasColumnName("compare_at_price")
            .HasPrecision(15, 2);

        builder.Property(p => p.CostPrice)
            .HasColumnName("cost_price")
            .HasPrecision(15, 2);

        // Stock
        builder.Property(p => p.Stock)
            .HasColumnName("stock")
            .HasDefaultValue(0);

        builder.Property(p => p.ReservedStock)
            .HasColumnName("reserved_stock")
            .HasDefaultValue(0);

        builder.Property(p => p.LowStockThreshold)
            .HasColumnName("low_stock_threshold")
            .HasDefaultValue(10);

        // Dimensions
        builder.Property(p => p.WeightGrams)
            .HasColumnName("weight_grams");

        builder.Property(p => p.HeightCm)
            .HasColumnName("height_cm")
            .HasPrecision(10, 2);

        builder.Property(p => p.WidthCm)
            .HasColumnName("width_cm")
            .HasPrecision(10, 2);

        builder.Property(p => p.LengthCm)
            .HasColumnName("length_cm")
            .HasPrecision(10, 2);

        // SEO
        builder.Property(p => p.MetaTitle)
            .HasColumnName("meta_title")
            .HasMaxLength(70);

        builder.Property(p => p.MetaDescription)
            .HasColumnName("meta_description")
            .HasMaxLength(160);

        // Status and Flags
        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(ProductStatus.Draft);

        builder.Property(p => p.IsFeatured)
            .HasColumnName("is_featured")
            .HasDefaultValue(false);

        builder.Property(p => p.IsDigital)
            .HasColumnName("is_digital")
            .HasDefaultValue(false);

        builder.Property(p => p.RequiresShipping)
            .HasColumnName("requires_shipping")
            .HasDefaultValue(true);

        // Flexible attributes
        builder.Property(p => p.Attributes)
            .HasColumnName("attributes")
            .HasColumnType("jsonb");

        builder.Property(p => p.Tags)
            .HasColumnName("tags")
            .HasColumnType("text[]");

        builder.Property(p => p.PublishedAt)
            .HasColumnName("published_at");

        // Audit fields
        builder.Property(p => p.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at");

        // Indexes
        builder.HasIndex(p => p.Sku)
            .IsUnique();

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.HasIndex(p => p.CategoryId);

        builder.HasIndex(p => p.BrandId);

        builder.HasIndex(p => p.Status);

        builder.HasIndex(p => p.Tags)
            .HasMethod("gin");

        // Relationships
        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
