namespace Catalog.Domain.Enums;

/// <summary>
/// Tipos de movimentação de estoque.
/// Espelha o enum shared.stock_movement_type do banco de dados.
/// </summary>
public enum StockMovementType
{
    /// <summary>
    /// Entrada de estoque (compra, devolução, etc.).
    /// </summary>
    In = 0,

    /// <summary>
    /// Saída de estoque (venda, perda, etc.).
    /// </summary>
    Out = 1,

    /// <summary>
    /// Ajuste de estoque (inventário, correção).
    /// </summary>
    Adjustment = 2,

    /// <summary>
    /// Reserva de estoque (carrinho, pedido pendente).
    /// </summary>
    Reserve = 3,

    /// <summary>
    /// Liberação de reserva (cancelamento, expiração).
    /// </summary>
    Release = 4
}
