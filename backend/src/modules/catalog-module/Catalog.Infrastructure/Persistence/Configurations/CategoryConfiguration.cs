using Catalog.Domain.Aggregates.Category;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração da entidade Category para o Entity Framework Core.
/// Mapeia a tabela catalog.categories.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(c => c.ParentId)
            .HasColumnName("parent_id");

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("description");

        builder.Property(c => c.ImageUrl)
            .HasColumnName("image_url");

        builder.Property(c => c.Path)
            .HasColumnName("path")
            .HasMaxLength(1000);

        builder.Property(c => c.Depth)
            .HasColumnName("depth")
            .HasDefaultValue(0);

        builder.Property(c => c.MetaTitle)
            .HasColumnName("meta_title")
            .HasMaxLength(70);

        builder.Property(c => c.MetaDescription)
            .HasColumnName("meta_description")
            .HasMaxLength(160);

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(c => c.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0);

        // Audit fields
        builder.Property(c => c.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at");

        // Indexes
        builder.HasIndex(c => c.Slug)
            .IsUnique();

        builder.HasIndex(c => c.ParentId);

        builder.HasIndex(c => c.Path);

        // Self-referencing relationship
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
