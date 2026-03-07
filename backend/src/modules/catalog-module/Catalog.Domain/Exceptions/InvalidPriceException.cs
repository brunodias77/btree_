using Shared.Domain.Exceptions;

namespace Catalog.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um preço é inválido.
/// </summary>
public class InvalidPriceException : DomainException
{
    public decimal Price { get; }

    public InvalidPriceException(decimal price)
        : base("INVALID_PRICE", $"O preço '{price:C}' é inválido. O preço deve ser maior ou igual a zero.")
    {
        Price = price;
    }

    public InvalidPriceException(decimal price, decimal compareAtPrice)
        : base("INVALID_PRICE", $"O preço de comparação ({compareAtPrice:C}) deve ser maior que o preço de venda ({price:C}).")
    {
        Price = price;
    }
}
