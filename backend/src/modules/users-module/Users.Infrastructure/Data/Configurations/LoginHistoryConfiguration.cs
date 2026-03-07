using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Aggregates.LoginHistory;

namespace Users.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade LoginHistory para o Entity Framework Core.
/// Mapeia a tabela users.login_history.
/// </summary>
public class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
{
    public void Configure(EntityTypeBuilder<LoginHistory> builder)
    {
        builder.ToTable("login_history");

        builder.HasKey(lh => lh.Id);

        builder.Property(lh => lh.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(lh => lh.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(lh => lh.LoginProvider)
            .HasColumnName("login_provider")
            .HasMaxLength(50)
            .HasDefaultValue("Local")
            .IsRequired();

        builder.Property(lh => lh.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45);

        builder.Property(lh => lh.UserAgent)
            .HasColumnName("user_agent");

        builder.Property(lh => lh.Country)
            .HasColumnName("country")
            .HasMaxLength(2);

        builder.Property(lh => lh.City)
            .HasColumnName("city")
            .HasMaxLength(100);

        builder.Property(lh => lh.DeviceType)
            .HasColumnName("device_type")
            .HasMaxLength(20);

        builder.Property(lh => lh.DeviceInfo)
            .HasColumnName("device_info")
            .HasColumnType("jsonb");

        builder.Property(lh => lh.Success)
            .HasColumnName("success")
            .HasDefaultValue(true);

        builder.Property(lh => lh.FailureReason)
            .HasColumnName("failure_reason")
            .HasMaxLength(100);

        builder.Property(lh => lh.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Índices
        builder.HasIndex(lh => lh.UserId);
        builder.HasIndex(lh => lh.IpAddress);
    }
}
