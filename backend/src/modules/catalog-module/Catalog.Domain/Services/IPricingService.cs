using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Services;

/// <summary>
/// Interface de serviço de domínio para cálculos de preços.
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// Calcula o preço final de um produto considerando promoções ativas.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Informações do preço calculado.</returns>
    Task<PriceInfo> CalculatePriceAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula o preço para uma quantidade específica.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="quantity">Quantidade desejada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Informações do preço calculado.</returns>
    Task<PriceInfo> CalculatePriceAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aplica um desconto percentual ao preço.
    /// </summary>
    /// <param name="originalPrice">Preço original.</param>
    /// <param name="discountPercentage">Percentual de desconto (0-100).</param>
    /// <returns>Preço com desconto aplicado.</returns>
    Money ApplyDiscount(Money originalPrice, decimal discountPercentage);

    /// <summary>
    /// Calcula o desconto entre dois preços.
    /// </summary>
    /// <param name="originalPrice">Preço original (compare at price).</param>
    /// <param name="currentPrice">Preço atual.</param>
    /// <returns>Percentual de desconto.</returns>
    decimal CalculateDiscountPercentage(Money originalPrice, Money currentPrice);

    /// <summary>
    /// Verifica se um produto tem promoção ativa.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se há promoção ativa.</returns>
    Task<bool> HasActivePromotionAsync(Guid productId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Informações de preço calculado.
/// </summary>
public sealed record PriceInfo
{
    /// <summary>
    /// Preço unitário original.
    /// </summary>
    public required Money UnitPrice { get; init; }

    /// <summary>
    /// Preço de comparação (preço "de").
    /// </summary>
    public Money? CompareAtPrice { get; init; }

    /// <summary>
    /// Preço final após descontos.
    /// </summary>
    public required Money FinalPrice { get; init; }

    /// <summary>
    /// Quantidade.
    /// </summary>
    public int Quantity { get; init; } = 1;

    /// <summary>
    /// Preço total (FinalPrice * Quantity).
    /// </summary>
    public required Money TotalPrice { get; init; }

    /// <summary>
    /// Percentual de desconto aplicado.
    /// </summary>
    public decimal DiscountPercentage { get; init; }

    /// <summary>
    /// Valor economizado.
    /// </summary>
    public Money? Savings { get; init; }

    /// <summary>
    /// Indica se há desconto aplicado.
    /// </summary>
    public bool HasDiscount => DiscountPercentage > 0;

    /// <summary>
    /// Nome da promoção aplicada (se houver).
    /// </summary>
    public string? PromotionName { get; init; }
}
