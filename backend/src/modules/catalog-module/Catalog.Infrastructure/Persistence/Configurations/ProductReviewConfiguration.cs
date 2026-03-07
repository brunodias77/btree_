using Catalog.Domain.Aggregates.ProductReview;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração da entidade ProductReview para o Entity Framework Core.
/// Mapeia a tabela catalog.product_reviews.
/// </summary>
public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("product_reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(r => r.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(r => r.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(r => r.OrderId)
            .HasColumnName("order_id");

        builder.Property(r => r.Rating)
            .HasColumnName("rating")
            .IsRequired();

        builder.Property(r => r.Title)
            .HasColumnName("title")
            .HasMaxLength(200);

        builder.Property(r => r.Comment)
            .HasColumnName("comment");

        builder.Property(r => r.IsVerifiedPurchase)
            .HasColumnName("is_verified_purchase")
            .HasDefaultValue(false);

        builder.Property(r => r.Status)
            .HasColumnName("is_approved")
            .HasConversion(
                v => v == ReviewStatus.Approved,
                v => v ? ReviewStatus.Approved : ReviewStatus.Pending);

        builder.Property(r => r.SellerResponse)
            .HasColumnName("seller_response");

        builder.Property(r => r.SellerRespondedAt)
            .HasColumnName("seller_responded_at");

        // Audit fields
        builder.Property(r => r.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(r => r.DeletedAt)
            .HasColumnName("deleted_at");

        // Unique constraint (each user can review a product only once)
        builder.HasIndex(r => new { r.ProductId, r.UserId })
            .IsUnique();

        // Indexes
        builder.HasIndex(r => r.ProductId);

        builder.HasIndex(r => r.UserId);
    }
}
