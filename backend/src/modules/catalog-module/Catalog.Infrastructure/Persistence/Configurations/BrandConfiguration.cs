using Catalog.Domain.Aggregates.Brand;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração da entidade Brand para o Entity Framework Core.
/// Mapeia a tabela catalog.brands.
/// </summary>
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(b => b.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasColumnName("description");

        builder.Property(b => b.LogoUrl)
            .HasColumnName("logo_url");

        builder.Property(b => b.WebsiteUrl)
            .HasColumnName("website_url");

        builder.Property(b => b.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(b => b.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0);

        // Audit fields
        builder.Property(b => b.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(b => b.DeletedAt)
            .HasColumnName("deleted_at");

        // Indexes
        builder.HasIndex(b => b.Slug)
            .IsUnique();

        builder.HasIndex(b => b.Name);
    }
}
