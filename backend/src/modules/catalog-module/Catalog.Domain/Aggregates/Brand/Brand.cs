using Catalog.Domain.Aggregates.Brand.Events;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.Aggregates.Brand;

/// <summary>
/// Aggregate Root representando uma marca de produtos do catálogo.
/// </summary>
public sealed class Brand : AggregateRoot<Guid>
{
    /// <summary>
    /// Nome da marca.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Slug da marca para URL amigável.
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Descrição da marca.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// URL do logo da marca.
    /// </summary>
    public string? LogoUrl { get; private set; }

    /// <summary>
    /// URL do website oficial da marca.
    /// </summary>
    public string? WebsiteUrl { get; private set; }

    /// <summary>
    /// Indica se a marca está ativa.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Ordem de exibição da marca.
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Construtor privado para EF Core.
    /// </summary>
    private Brand() : base() { }

    /// <summary>
    /// Construtor privado para criação da marca.
    /// </summary>
    private Brand(Guid id, string name, string slug) : base(id)
    {
        Name = name;
        Slug = slug;
    }

    /// <summary>
    /// Factory method para criar uma nova marca.
    /// </summary>
    /// <param name="name">Nome da marca.</param>
    /// <param name="slug">Slug para URL.</param>
    /// <param name="description">Descrição da marca (opcional).</param>
    /// <param name="logoUrl">URL do logo (opcional).</param>
    /// <param name="websiteUrl">URL do website (opcional).</param>
    /// <returns>Nova instância de Brand.</returns>
    public static Brand Create(
        string name,
        string slug,
        string? description = null,
        string? logoUrl = null,
        string? websiteUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome da marca é obrigatório.", nameof(name));
        }

        var brand = new Brand(Guid.NewGuid(), name.Trim(), slug)
        {
            Description = description,
            LogoUrl = logoUrl,
            WebsiteUrl = websiteUrl
        };

        brand.RegisterDomainEvent(new BrandCreatedDomainEvent(
            brand.Id,
            brand.Name,
            brand.Slug));

        return brand;
    }

    /// <summary>
    /// Atualiza os dados da marca.
    /// </summary>
    /// <param name="name">Novo nome.</param>
    /// <param name="slug">Novo slug.</param>
    /// <param name="description">Nova descrição.</param>
    /// <param name="logoUrl">Nova URL do logo.</param>
    /// <param name="websiteUrl">Nova URL do website.</param>
    /// <param name="sortOrder">Nova ordem de exibição.</param>
    public void Update(
        string name,
        string slug,
        string? description = null,
        string? logoUrl = null,
        string? websiteUrl = null,
        int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome da marca é obrigatório.", nameof(name));
        }

        Name = name.Trim();
        Slug = slug;
        Description = description;
        LogoUrl = logoUrl;
        WebsiteUrl = websiteUrl;
        SortOrder = sortOrder;
        IncrementVersion();

        RegisterDomainEvent(new BrandUpdatedDomainEvent(Id, Name, Slug));
    }

    /// <summary>
    /// Atualiza apenas o logo da marca.
    /// </summary>
    /// <param name="logoUrl">Nova URL do logo.</param>
    public void UpdateLogo(string? logoUrl)
    {
        LogoUrl = logoUrl;
        IncrementVersion();
    }

    /// <summary>
    /// Ativa a marca.
    /// </summary>
    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            IncrementVersion();
        }
    }

    /// <summary>
    /// Desativa a marca.
    /// </summary>
    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            IncrementVersion();
        }
    }

    /// <summary>
    /// Marca a marca como excluída (soft delete) e dispara evento de domínio.
    /// </summary>
    public override void Delete()
    {
        if (IsDeleted) return;

        base.Delete();
        IsActive = false;
        IncrementVersion();

        RegisterDomainEvent(new BrandDeletedDomainEvent(Id, Name));
    }

    /// <summary>
    /// Restaura uma marca excluída.
    /// </summary>
    public override void Restore()
    {
        if (!IsDeleted) return;

        base.Restore();
        IsActive = true;
        IncrementVersion();
    }
}
