using Catalog.Domain.Aggregates.Product.Entities;
using Catalog.Domain.Aggregates.Product.Events;
using Catalog.Domain.Enums;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.Aggregates.Product;

/// <summary>
/// Aggregate Root representando um produto do catálogo.
/// </summary>
public sealed class Product : AggregateRoot<Guid>
{
    private readonly List<ProductImage> _images = new();

    /// <summary>
    /// ID da categoria do produto.
    /// </summary>
    public Guid? CategoryId { get; private set; }

    /// <summary>
    /// ID da marca do produto.
    /// </summary>
    public Guid? BrandId { get; private set; }

    /// <summary>
    /// SKU (Stock Keeping Unit) - código único do produto.
    /// </summary>
    public string Sku { get; private set; } = string.Empty;

    /// <summary>
    /// Slug do produto para URL amigável.
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Código de barras do produto.
    /// </summary>
    public string? Barcode { get; private set; }

    /// <summary>
    /// Nome do produto.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Descrição curta do produto (máx. 500 caracteres).
    /// </summary>
    public string? ShortDescription { get; private set; }

    /// <summary>
    /// Descrição completa do produto.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Preço de venda do produto.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Preço de comparação (preço anterior/riscado).
    /// </summary>
    public decimal? CompareAtPrice { get; private set; }

    /// <summary>
    /// Preço de custo do produto.
    /// </summary>
    public decimal? CostPrice { get; private set; }

    /// <summary>
    /// Quantidade em estoque.
    /// </summary>
    public int Stock { get; private set; }

    /// <summary>
    /// Quantidade reservada em carrinhos/pedidos pendentes.
    /// </summary>
    public int ReservedStock { get; private set; }

    /// <summary>
    /// Estoque disponível (Stock - ReservedStock).
    /// </summary>
    public int AvailableStock => Stock - ReservedStock;

    /// <summary>
    /// Limite para alerta de estoque baixo.
    /// </summary>
    public int LowStockThreshold { get; private set; } = 10;

    /// <summary>
    /// Peso em gramas.
    /// </summary>
    public int? WeightGrams { get; private set; }

    /// <summary>
    /// Altura em centímetros.
    /// </summary>
    public decimal? HeightCm { get; private set; }

    /// <summary>
    /// Largura em centímetros.
    /// </summary>
    public decimal? WidthCm { get; private set; }

    /// <summary>
    /// Comprimento em centímetros.
    /// </summary>
    public decimal? LengthCm { get; private set; }

    /// <summary>
    /// Título para SEO (máx. 70 caracteres).
    /// </summary>
    public string? MetaTitle { get; private set; }

    /// <summary>
    /// Descrição para SEO (máx. 160 caracteres).
    /// </summary>
    public string? MetaDescription { get; private set; }

    /// <summary>
    /// Status atual do produto.
    /// </summary>
    public ProductStatus Status { get; private set; } = ProductStatus.Draft;

    /// <summary>
    /// Indica se o produto é destaque.
    /// </summary>
    public bool IsFeatured { get; private set; }

    /// <summary>
    /// Indica se o produto é digital (não requer envio físico).
    /// </summary>
    public bool IsDigital { get; private set; }

    /// <summary>
    /// Indica se o produto requer envio.
    /// </summary>
    public bool RequiresShipping { get; private set; } = true;

    /// <summary>
    /// Atributos flexíveis do produto (JSON).
    /// </summary>
    public string? Attributes { get; private set; }

    /// <summary>
    /// Tags do produto.
    /// </summary>
    public List<string> Tags { get; private set; } = new();

    /// <summary>
    /// Data/hora de publicação.
    /// </summary>
    public DateTime? PublishedAt { get; private set; }

    /// <summary>
    /// Imagens do produto (coleção somente leitura).
    /// </summary>
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    /// <summary>
    /// Construtor privado para EF Core.
    /// </summary>
    private Product() : base() { }

    /// <summary>
    /// Construtor privado para criação do produto.
    /// </summary>
    private Product(
        Guid id,
        string sku,
        string slug,
        string name,
        decimal price) : base(id)
    {
        Sku = sku;
        Slug = slug;
        Name = name;
        Price = price;
    }

    /// <summary>
    /// Factory method para criar um novo produto.
    /// </summary>
    public static Product Create(
        string sku,
        string slug,
        string name,
        decimal price,
        Guid? categoryId = null,
        Guid? brandId = null)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("O SKU é obrigatório.", nameof(sku));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(name));
        
        if (price < 0)
            throw new ArgumentException("O preço não pode ser negativo.", nameof(price));

        var product = new Product(Guid.NewGuid(), sku.Trim().ToUpperInvariant(), slug, name.Trim(), price)
        {
            CategoryId = categoryId,
            BrandId = brandId
        };

        product.RegisterDomainEvent(new ProductCreatedDomainEvent(
            product.Id,
            product.Sku,
            product.Name,
            product.CategoryId,
            product.BrandId));

        return product;
    }

    /// <summary>
    /// Atualiza os dados básicos do produto.
    /// </summary>
    public void Update(
        string name,
        string slug,
        string? shortDescription = null,
        string? description = null,
        string? barcode = null,
        string? metaTitle = null,
        string? metaDescription = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(name));

        Name = name.Trim();
        Slug = slug;
        ShortDescription = shortDescription;
        Description = description;
        Barcode = barcode;
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        IncrementVersion();

        RegisterDomainEvent(new ProductUpdatedDomainEvent(Id, Name, Slug));
    }

    /// <summary>
    /// Atualiza o SKU do produto.
    /// </summary>
    public void UpdateSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("O SKU é obrigatório.", nameof(sku));

        if (Sku == sku)
            return;

        Sku = sku.Trim().ToUpperInvariant();
        IncrementVersion();
    }

    /// <summary>
    /// Atualiza os preços do produto.
    /// </summary>
    public void UpdatePrice(decimal newPrice, decimal? newCompareAtPrice = null, decimal? newCostPrice = null)
    {
        if (newPrice < 0)
            throw new ArgumentException("O preço não pode ser negativo.", nameof(newPrice));

        if (newCompareAtPrice.HasValue && newCompareAtPrice <= newPrice)
            throw new ArgumentException("O preço de comparação deve ser maior que o preço de venda.", nameof(newCompareAtPrice));

        var oldPrice = Price;
        var oldCompareAtPrice = CompareAtPrice;

        Price = newPrice;
        CompareAtPrice = newCompareAtPrice;
        CostPrice = newCostPrice;
        IncrementVersion();

        RegisterDomainEvent(new ProductPriceChangedDomainEvent(
            Id, Sku, oldPrice, newPrice, oldCompareAtPrice, newCompareAtPrice));
    }

    /// <summary>
    /// Atualiza o estoque do produto.
    /// </summary>
    public void UpdateStock(int newStock, string? reason = null)
    {
        if (newStock < 0)
            throw new ArgumentException("O estoque não pode ser negativo.", nameof(newStock));

        var oldStock = Stock;
        var wasOutOfStock = Stock == 0;
        
        Stock = newStock;
        IncrementVersion();

        RegisterDomainEvent(new ProductStockChangedDomainEvent(
            Id, Sku, oldStock, newStock, reason));

        // Verificar se ficou sem estoque
        if (newStock == 0 && !wasOutOfStock)
        {
            Status = ProductStatus.OutOfStock;
            RegisterDomainEvent(new ProductOutOfStockDomainEvent(Id, Sku, Name));
        }
        // Verificar se voltou a ter estoque
        else if (newStock > 0 && wasOutOfStock)
        {
            if (Status == ProductStatus.OutOfStock)
            {
                Status = ProductStatus.Active;
            }
            RegisterDomainEvent(new ProductBackInStockDomainEvent(Id, Sku, Name, newStock));
        }
    }

    /// <summary>
    /// Reserva estoque para um pedido/carrinho.
    /// </summary>
    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade a reservar deve ser maior que zero.", nameof(quantity));

        if (AvailableStock < quantity)
            throw new InvalidOperationException($"Estoque insuficiente. Disponível: {AvailableStock}, Solicitado: {quantity}");

        ReservedStock += quantity;
        IncrementVersion();
    }

    /// <summary>
    /// Libera estoque reservado.
    /// </summary>
    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade a liberar deve ser maior que zero.", nameof(quantity));

        if (ReservedStock < quantity)
            throw new InvalidOperationException("Quantidade a liberar maior que o estoque reservado.");

        ReservedStock -= quantity;
        IncrementVersion();
    }

    /// <summary>
    /// Confirma reserva de estoque (decrementa estoque e reserva).
    /// </summary>
    public void ConfirmReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantity));

        if (ReservedStock < quantity)
            throw new InvalidOperationException("Quantidade a confirmar maior que o estoque reservado.");

        Stock -= quantity;
        ReservedStock -= quantity;
        IncrementVersion();

        if (Stock == 0)
        {
            Status = ProductStatus.OutOfStock;
            RegisterDomainEvent(new ProductOutOfStockDomainEvent(Id, Sku, Name));
        }
    }

    /// <summary>
    /// Atualiza as dimensões do produto.
    /// </summary>
    public void UpdateDimensions(int? weightGrams, decimal? heightCm, decimal? widthCm, decimal? lengthCm)
    {
        WeightGrams = weightGrams;
        HeightCm = heightCm;
        WidthCm = widthCm;
        LengthCm = lengthCm;
        IncrementVersion();
    }

    /// <summary>
    /// Atualiza a categoria do produto.
    /// </summary>
    public void SetCategory(Guid? categoryId)
    {
        CategoryId = categoryId;
        IncrementVersion();
    }

    /// <summary>
    /// Atualiza a marca do produto.
    /// </summary>
    public void SetBrand(Guid? brandId)
    {
        BrandId = brandId;
        IncrementVersion();
    }

    /// <summary>
    /// Define se o produto é destaque.
    /// </summary>
    public void SetFeatured(bool isFeatured)
    {
        if (IsFeatured != isFeatured)
        {
            IsFeatured = isFeatured;
            IncrementVersion();
        }
    }

    /// <summary>
    /// Publica o produto (torna disponível para venda).
    /// </summary>
    public void Publish()
    {
        if (Status == ProductStatus.Active)
            return;

        Status = Stock > 0 ? ProductStatus.Active : ProductStatus.OutOfStock;
        PublishedAt = DateTime.UtcNow;
        IncrementVersion();

        RegisterDomainEvent(new ProductPublishedDomainEvent(Id, Sku, Name, PublishedAt.Value));
    }

    /// <summary>
    /// Despublica o produto (torna indisponível para venda).
    /// </summary>
    public void Unpublish()
    {
        if (Status == ProductStatus.Inactive || Status == ProductStatus.Draft)
            return;

        Status = ProductStatus.Inactive;
        IncrementVersion();
    }

    /// <summary>
    /// Descontinua o produto.
    /// </summary>
    public void Discontinue()
    {
        Status = ProductStatus.Discontinued;
        IncrementVersion();
    }

    /// <summary>
    /// Adiciona uma imagem ao produto.
    /// </summary>
    public ProductImage AddImage(string url, string? altText = null, bool isPrimary = false)
    {
        // Se é a primeira imagem ou marcada como primária, definir como primária
        if (_images.Count == 0 || isPrimary)
        {
            // Desmarcar outras como primária
            foreach (var img in _images)
            {
                img.UnsetAsPrimary();
            }
            isPrimary = true;
        }

        var sortOrder = _images.Count;
        var image = ProductImage.Create(Id, url, altText, isPrimary, sortOrder);
        _images.Add(image);
        IncrementVersion();

        RegisterDomainEvent(new ProductImageAddedDomainEvent(Id, image.Id, url, isPrimary));

        return image;
    }

    /// <summary>
    /// Remove uma imagem do produto.
    /// </summary>
    public void RemoveImage(Guid imageId)
    {
        var image = _images.Find(i => i.Id == imageId);
        if (image == null) return;

        var wasPrimary = image.IsPrimary;
        var imageUrlOutput = image.Url;

        _images.Remove(image);

        // Se removeu a primária, definir a primeira como primária
        if (wasPrimary && _images.Count > 0)
        {
            _images[0].SetAsPrimary();
        }

        IncrementVersion();

        RegisterDomainEvent(new ProductImageRemovedDomainEvent(Id, imageId, imageUrlOutput));
    }

    /// <summary>
    /// Define uma imagem como primária.
    /// </summary>
    public void SetPrimaryImage(Guid imageId)
    {
        foreach (var img in _images)
        {
            if (img.Id == imageId)
                img.SetAsPrimary();
            else
                img.UnsetAsPrimary();
        }
        IncrementVersion();
    }

    /// <summary>
    /// Marca o produto como excluído (soft delete) e dispara evento de domínio.
    /// </summary>
    public override void Delete()
    {
        if (IsDeleted) return;

        base.Delete();
        Status = ProductStatus.Discontinued;
        IncrementVersion();

        RegisterDomainEvent(new ProductDeletedDomainEvent(Id, Sku, Name));
    }

    /// <summary>
    /// Restaura um produto excluído.
    /// </summary>
    public override void Restore()
    {
        if (!IsDeleted) return;

        base.Restore();
        Status = Stock > 0 ? ProductStatus.Inactive : ProductStatus.OutOfStock;
        IncrementVersion();
    }
}
