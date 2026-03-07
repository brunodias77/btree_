using Catalog.Domain.Aggregates.UserFavorite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração da entidade UserFavorite para o Entity Framework Core.
/// Mapeia a tabela catalog.user_favorites.
/// </summary>
public class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
{
    public void Configure(EntityTypeBuilder<UserFavorite> builder)
    {
        builder.ToTable("user_favorites");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(f => f.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(f => f.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(f => f.ProductSnapshot)
            .HasColumnName("product_snapshot")
            .HasColumnType("jsonb");

        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Unique constraint (each user can favorite a product only once)
        builder.HasIndex(f => new { f.UserId, f.ProductId })
            .IsUnique();

        // Indexes
        builder.HasIndex(f => f.UserId);

        builder.HasIndex(f => f.ProductId);
    }
}
