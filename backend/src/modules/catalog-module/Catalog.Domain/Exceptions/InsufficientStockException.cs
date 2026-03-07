using Shared.Domain.Exceptions;

namespace Catalog.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando não há estoque suficiente para a operação.
/// </summary>
public class InsufficientStockException : DomainException
{
    public Guid ProductId { get; }
    public string? Sku { get; }
    public int RequestedQuantity { get; }
    public int AvailableQuantity { get; }

    public InsufficientStockException(Guid productId, int requestedQuantity, int availableQuantity)
        : base("INSUFFICIENT_STOCK", 
            $"Estoque insuficiente para o produto {productId}. Solicitado: {requestedQuantity}, Disponível: {availableQuantity}.")
    {
        ProductId = productId;
        RequestedQuantity = requestedQuantity;
        AvailableQuantity = availableQuantity;
    }

    public InsufficientStockException(string sku, int requestedQuantity, int availableQuantity)
        : base("INSUFFICIENT_STOCK", 
            $"Estoque insuficiente para o produto '{sku}'. Solicitado: {requestedQuantity}, Disponível: {availableQuantity}.")
    {
        Sku = sku;
        RequestedQuantity = requestedQuantity;
        AvailableQuantity = availableQuantity;
    }
}
