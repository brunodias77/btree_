using BuildingBlocks.Domain.Abstractions;
using Users.Domain.Aggregates.Address.Events;

namespace Users.Domain.Aggregates.Addresses;

/// <summary>
/// Endereço de entrega ou cobrança do usuário.
/// </summary>
public sealed class Address : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }

    // Dados do endereço
    public string? Label { get; private set; }
    public string? RecipientName { get; private set; }
    public string Street { get; private set; } = null!;
    public string? Number { get; private set; }
    public string? Complement { get; private set; }
    public string? Neighborhood { get; private set; }
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;
    public string Country { get; private set; } = "BR";

    // Coordenadas
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public string? IbgeCode { get; private set; }

    // Controle
    public bool IsDefault { get; private set; }
    public bool IsBillingAddress { get; private set; }
    // Propriedades CreatedAt, UpdatedAt, DeletedAt herdadas de Entity<TId>

    private Address() : base() { }

    private Address(
        Guid id,
        Guid userId,
        string street,
        string city,
        string state,
        string postalCode) : base(id)
    {
        UserId = userId;
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        // CreatedAt inicializado na base
    }

    public static Address Create(
        Guid userId,
        string street,
        string city,
        string state,
        string postalCode,
        string? label = null,
        string? recipientName = null,
        string? number = null,
        string? complement = null,
        string? neighborhood = null,
        string country = "BR",
        bool isDefault = false,
        bool isBillingAddress = false)
    {
        var address = new Address(Guid.NewGuid(), userId, street, city, state, postalCode)
        {
            Label = label,
            RecipientName = recipientName,
            Number = number,
            Complement = complement,
            Neighborhood = neighborhood,
            Country = country,
            IsDefault = isDefault,
            IsBillingAddress = isBillingAddress
        };

        address.RaiseDomainEvent(new AddressCreatedDomainEvent(address.Id, userId));

        return address;
    }

    public void Update(
        string? label,
        string? recipientName,
        string street,
        string? number,
        string? complement,
        string? neighborhood,
        string city,
        string state,
        string postalCode,
        string? country,
        bool? isBillingAddress)
    {
        Label = label;
        RecipientName = recipientName;
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country ?? Country;
        IsBillingAddress = isBillingAddress ?? IsBillingAddress;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new AddressUpdatedDomainEvent(Id, UserId));
    }

    public void SetCoordinates(decimal latitude, decimal longitude, string? ibgeCode = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        IbgeCode = ibgeCode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsDefault()
    {
        IsDefault = true;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new AddressSetAsDefaultDomainEvent(Id, UserId));
    }

    public void RemoveDefault()
    {
        IsDefault = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsBillingAddress()
    {
        IsBillingAddress = true;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new AddressUpdatedDomainEvent(Id, UserId));
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDefault = false;

        RaiseDomainEvent(new AddressDeletedDomainEvent(Id, UserId));
    }

    public string GetFormattedAddress()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(Street))
        {
            var streetPart = Street;
            if (!string.IsNullOrWhiteSpace(Number))
                streetPart += $", {Number}";
            if (!string.IsNullOrWhiteSpace(Complement))
                streetPart += $" - {Complement}";
            parts.Add(streetPart);
        }

        if (!string.IsNullOrWhiteSpace(Neighborhood))
            parts.Add(Neighborhood);

        parts.Add($"{City} - {State}");
        parts.Add(PostalCode);

        return string.Join(", ", parts);
    }
}
