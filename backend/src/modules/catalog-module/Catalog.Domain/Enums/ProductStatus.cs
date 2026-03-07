namespace Catalog.Domain.Enums;

/// <summary>
/// Status do produto no catálogo.
/// Espelha o enum shared.product_status do banco de dados.
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// Rascunho - produto ainda não publicado.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Ativo - produto disponível para venda.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Inativo - produto temporariamente indisponível.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Sem estoque - produto esgotado.
    /// </summary>
    OutOfStock = 3,

    /// <summary>
    /// Descontinuado - produto não será mais vendido.
    /// </summary>
    Discontinued = 4
}
