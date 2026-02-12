using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Aggregates.Profile;

namespace Users.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Profile para o Entity Framework Core.
/// Mapeia a tabela users.profiles.
/// </summary>
public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        // Define o nome da tabela
        builder.ToTable("profiles");

        // Chave primária
        builder.HasKey(p => p.Id);

        // Propriedade Id
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Foreign Key para ApplicationUser
        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();
            
        // Índice único para user_id (relação 1:1)
        builder.HasIndex(p => p.UserId)
            .IsUnique();

        // Dados Pessoais
        builder.Property(p => p.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100);

        builder.Property(p => p.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(100);

        builder.Property(p => p.AvatarUrl)
            .HasColumnName("avatar_url");

        builder.Property(p => p.BirthDate)
            .HasColumnName("birth_date");

        builder.Property(p => p.Gender)
            .HasColumnName("gender")
            .HasMaxLength(20);

        builder.Property(p => p.Cpf)
            .HasColumnName("cpf")
            .HasMaxLength(14); // Formato 000.000.000-00

        builder.Property(p => p.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20);

        // Preferências
        builder.Property(p => p.PreferredLanguage)
            .HasColumnName("preferred_language")
            .HasMaxLength(5)
            .HasDefaultValue("pt-BR");

        builder.Property(p => p.PreferredCurrency)
            .HasColumnName("preferred_currency")
            .HasMaxLength(3)
            .HasDefaultValue("BRL");

        builder.Property(p => p.NewsletterSubscribed)
            .HasColumnName("newsletter_subscribed")
            .HasDefaultValue(false);

        // Termos
        builder.Property(p => p.AcceptedTermsAt)
            .HasColumnName("accepted_terms_at");

        builder.Property(p => p.AcceptedPrivacyAt)
            .HasColumnName("accepted_privacy_at");

        // Controle (Audit)
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

        // O DB tem constraints CHECK que são definidas no SQL, não precisa repetir aqui
        // a menos que queira validação no lado cliente, mas EF Migrations pode tentar criar duplicado.
        // Vamos confiar no schema.sql para os CHECKS.
    }
}
