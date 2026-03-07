using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Outbox;

namespace Shared.Infrastructure.Data.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("domain_events", "shared");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(o => o.Module)
            .HasColumnName("module")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.AggregateType)
            .HasColumnName("aggregate_type")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(o => o.AggregateId)
            .HasColumnName("aggregate_id")
            .IsRequired();

        builder.Property(o => o.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(o => o.Payload)
            .HasColumnName("payload")
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(o => o.ProcessedAt)
            .HasColumnName("processed_at");

        builder.Property(o => o.ErrorMessage)
            .HasColumnName("error_message");

        builder.Property(o => o.RetryCount)
            .HasColumnName("retry_count")
            .IsRequired();
    }
}