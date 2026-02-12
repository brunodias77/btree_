using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Shared.IntegrationEvents.Users;
using Users.Domain.Aggregates.Profile.Events;

namespace Users.Domain.Aggregates.Profile;

/// <summary>
/// Perfil estendido do usuário.
/// Contém dados pessoais e preferências que complementam o Identity.
/// </summary>
public sealed class Profile : AggregateRoot<Guid>
{
    // Dados pessoais
    public Guid UserId { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? DisplayName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? Gender { get; private set; }
    public string? Cpf { get; private set; }
    // [NEW] Added for UpdateProfile feature
    public string? PhoneNumber { get; private set; }

    // Preferências
    public string PreferredLanguage { get; private set; } = "pt-BR";
    public string PreferredCurrency { get; private set; } = "BRL";
    public bool NewsletterSubscribed { get; private set; }

    // Termos
    public DateTime? AcceptedTermsAt { get; private set; }
    public DateTime? AcceptedPrivacyAt { get; private set; }

    // Controle
    // Version herdado de AggregateRoot<TId>
    // CreatedAt, UpdatedAt, DeletedAt herdados de Entity<TId>

    private Profile() : base() { }

    private Profile(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
        // CreatedAt inicializado na base
    }

    public static Profile Create(Guid userId, string email, string name, string? firstName = null, string? lastName = null)
    {
        var profile = new Profile(Guid.NewGuid(), userId)
        {
            FirstName = firstName,
            LastName = lastName,
            DisplayName = name
        };

        profile.RaiseDomainEvent(new ProfileCreatedDomainEvent(profile.Id, userId, email, name));

        return profile;
    }

    /// <summary>
    /// Updates core profile information using robust Value Objects.
    /// </summary>
    public void UpdateInfo(
        Users.Domain.ValueObjects.FullName fullName,
        Users.Domain.ValueObjects.PhoneNumber phoneNumber,
        string gender,
        DateTime birthDate)
    {
        FirstName = fullName.FirstName;
        LastName = fullName.LastName;
        PhoneNumber = phoneNumber.Value;
        Gender = gender;
        // Converting DateTime to DateOnly for storage
        BirthDate = DateOnly.FromDateTime(birthDate); 
        
        IncrementVersion();

        RaiseDomainEvent(new ProfileUpdatedDomainEvent(Id, UserId));
    }

    public void Update(
        string? firstName,
        string? lastName,
        string? displayName,
        DateOnly? birthDate,
        string? gender,
        string? cpf,
        string? preferredLanguage,
        string? preferredCurrency,
        bool? newsletterSubscribed)
    {
        FirstName = firstName ?? FirstName;
        LastName = lastName ?? LastName;
        DisplayName = displayName ?? DisplayName;
        BirthDate = birthDate ?? BirthDate;
        Gender = gender ?? Gender;
        Cpf = cpf ?? Cpf;
        PreferredLanguage = preferredLanguage ?? PreferredLanguage;
        PreferredCurrency = preferredCurrency ?? PreferredCurrency;
        NewsletterSubscribed = newsletterSubscribed ?? NewsletterSubscribed;
        
        IncrementVersion();

        RaiseDomainEvent(new ProfileUpdatedDomainEvent(Id, UserId));
    }

    public void UpdateAvatar(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
        IncrementVersion();

        RaiseDomainEvent(new ProfileUpdatedDomainEvent(Id, UserId));
    }

    public void AcceptTerms()
    {
        AcceptedTermsAt = DateTime.UtcNow;
        IncrementVersion();
    }

    public void AcceptPrivacy()
    {
        AcceptedPrivacyAt = DateTime.UtcNow;
        IncrementVersion();
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFullName()
    {
        if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
            return DisplayName ?? string.Empty;

        return $"{FirstName} {LastName}".Trim();
    }

}
