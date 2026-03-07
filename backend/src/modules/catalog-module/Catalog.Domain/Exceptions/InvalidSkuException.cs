using Shared.Domain.Exceptions;

namespace Catalog.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um SKU é inválido.
/// </summary>
public class InvalidSkuException : DomainException
{
    public string Sku { get; }

    public InvalidSkuException(string sku)
        : base("INVALID_SKU", $"O SKU '{sku}' é inválido. O SKU deve conter apenas letras, números, hífens e underscores.")
    {
        Sku = sku;
    }

    public InvalidSkuException(string sku, string reason)
        : base("INVALID_SKU", $"O SKU '{sku}' é inválido: {reason}")
    {
        Sku = sku;
    }
}
