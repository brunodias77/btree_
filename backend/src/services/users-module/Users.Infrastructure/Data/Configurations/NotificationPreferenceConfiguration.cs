using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Aggregates.NotificationPreference;

namespace Users.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade NotificationPreference para o Entity Framework Core.
/// Mapeia a tabela users.notification_preferences.
/// </summary>
public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");

        builder.HasKey(np => np.Id);

        builder.Property(np => np.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(np => np.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // Relacionamento 1:1 único com User
        builder.HasIndex(np => np.UserId)
            .IsUnique();

        builder.Property(np => np.EmailEnabled)
            .HasColumnName("email_enabled")
            .HasDefaultValue(true);

        builder.Property(np => np.PushEnabled)
            .HasColumnName("push_enabled")
            .HasDefaultValue(true);

        builder.Property(np => np.SmsEnabled)
            .HasColumnName("sms_enabled")
            .HasDefaultValue(false);

        builder.Property(np => np.OrderUpdates)
            .HasColumnName("order_updates")
            .HasDefaultValue(true);

        builder.Property(np => np.Promotions)
            .HasColumnName("promotions")
            .HasDefaultValue(true);

        builder.Property(np => np.PriceDrops)
            .HasColumnName("price_drops")
            .HasDefaultValue(true);

        builder.Property(np => np.BackInStock)
            .HasColumnName("back_in_stock")
            .HasDefaultValue(true);

        builder.Property(np => np.Newsletter)
            .HasColumnName("newsletter")
            .HasDefaultValue(false);

        builder.Property(np => np.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(np => np.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
