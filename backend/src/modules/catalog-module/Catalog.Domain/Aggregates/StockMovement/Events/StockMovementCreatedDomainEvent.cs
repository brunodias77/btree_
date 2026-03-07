using Shared.Domain.Events;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Aggregates.StockMovement.Events;

/// <summary>
/// Evento de domínio disparado quando uma movimentação de estoque é criada.
/// </summary>
/// <param name="MovementId">ID da movimentação.</param>
/// <param name="ProductId">ID do produto.</param>
/// <param name="MovementType">Tipo de movimentação.</param>
/// <param name="Quantity">Quantidade movimentada.</param>
/// <param name="StockBefore">Estoque antes da movimentação.</param>
/// <param name="StockAfter">Estoque após a movimentação.</param>
public sealed class StockMovementCreatedDomainEvent : DomainEventBase
{
    public Guid MovementId { get; }
    public Guid ProductId { get; }
    public StockMovementType MovementType { get; }
    public int Quantity { get; }
    public int StockBefore { get; }
    public int StockAfter { get; }

    public override string AggregateType => "StockMovement";
    public override Guid AggregateId => MovementId;
    public override string Module => "catalog";

    public StockMovementCreatedDomainEvent(Guid movementId, Guid productId, StockMovementType movementType, int quantity, int stockBefore, int stockAfter)
    {
        MovementId = movementId;
        ProductId = productId;
        MovementType = movementType;
        Quantity = quantity;
        StockBefore = stockBefore;
        StockAfter = stockAfter;
    }
}



