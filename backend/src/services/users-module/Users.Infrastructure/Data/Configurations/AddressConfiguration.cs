using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Aggregates.Address;
using Users.Domain.Aggregates.Addresses;

namespace Users.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Address para o Entity Framework Core.
/// Mapeia a tabela users.addresses.
/// </summary>
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(a => a.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // Dados do Endereço
        builder.Property(a => a.Label)
            .HasColumnName("label")
            .HasMaxLength(50);

        builder.Property(a => a.RecipientName)
            .HasColumnName("recipient_name")
            .HasMaxLength(150);

        builder.Property(a => a.Street)
            .HasColumnName("street")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Number)
            .HasColumnName("number")
            .HasMaxLength(20);

        builder.Property(a => a.Complement)
            .HasColumnName("complement")
            .HasMaxLength(100);

        builder.Property(a => a.Neighborhood)
            .HasColumnName("neighborhood")
            .HasMaxLength(100);

        builder.Property(a => a.City)
            .HasColumnName("city")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.State)
            .HasColumnName("state")
            .HasMaxLength(2)
            .IsFixedLength()
            .IsRequired();

        builder.Property(a => a.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(9)
            .IsRequired();

        builder.Property(a => a.Country)
            .HasColumnName("country")
            .HasMaxLength(2)
            .HasDefaultValue("BR")
            .IsRequired();

        // Coordenadas
        builder.Property(a => a.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(10, 8);

        builder.Property(a => a.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(11, 8);

        builder.Property(a => a.IbgeCode)
            .HasColumnName("ibge_code")
            .HasMaxLength(7);

        // Controle
        builder.Property(a => a.IsDefault)
            .HasColumnName("is_default")
            .HasDefaultValue(false);

        builder.Property(a => a.IsBillingAddress)
            .HasColumnName("is_billing_address")
            .HasDefaultValue(false);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(a => a.DeletedAt)
            .HasColumnName("deleted_at");

        // Índices para performance
        builder.HasIndex(a => a.UserId);
    }
}
