namespace Catalog.Domain.Enums;

/// <summary>
/// Tipos de referência para movimentações e reservas de estoque.
/// </summary>
public enum ReferenceType
{
    /// <summary>
    /// Referência a um carrinho de compras.
    /// </summary>
    Cart = 0,

    /// <summary>
    /// Referência a um pedido.
    /// </summary>
    Order = 1,

    /// <summary>
    /// Ajuste manual de estoque.
    /// </summary>
    Manual = 2,

    /// <summary>
    /// Importação de estoque.
    /// </summary>
    Import = 3,

    /// <summary>
    /// Devolução de produto.
    /// </summary>
    Return = 4
}
