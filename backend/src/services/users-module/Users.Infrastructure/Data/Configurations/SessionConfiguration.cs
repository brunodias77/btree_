using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Aggregates.Session;

namespace Users.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Session para o Entity Framework Core.
/// Mapeia a tabela users.sessions.
/// </summary>
public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(s => s.RefreshTokenHash)
            .HasColumnName("refresh_token_hash")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(s => s.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(100);

        builder.Property(s => s.DeviceName)
            .HasColumnName("device_name")
            .HasMaxLength(100);

        builder.Property(s => s.DeviceType)
            .HasColumnName("device_type")
            .HasMaxLength(20);

        builder.Property(s => s.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45);

        builder.Property(s => s.Country)
            .HasColumnName("country")
            .HasMaxLength(2);

        builder.Property(s => s.City)
            .HasColumnName("city")
            .HasMaxLength(100);

        builder.Property(s => s.IsCurrent)
            .HasColumnName("is_current")
            .HasDefaultValue(false);

        builder.Property(s => s.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(s => s.RevokedAt)
            .HasColumnName("revoked_at");

        builder.Property(s => s.RevokedReason)
            .HasColumnName("revoked_reason")
            .HasMaxLength(100);

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.LastActivityAt)
            .HasColumnName("last_activity_at")
            .IsRequired();

        // Índices
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.RefreshTokenHash);
    }
}
