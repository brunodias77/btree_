using Shared.Domain.Events;

namespace Catalog.Domain.Aggregates.StockReservation.Events;

/// <summary>
/// Evento de domínio disparado quando uma reserva de estoque é criada.
/// </summary>
/// <param name="ReservationId">ID da reserva.</param>
/// <param name="ProductId">ID do produto.</param>
/// <param name="ReferenceType">Tipo de referência (Cart, Order).</param>
/// <param name="ReferenceId">ID da referência.</param>
/// <param name="Quantity">Quantidade reservada.</param>
/// <param name="ExpiresAt">Data/hora de expiração.</param>
public sealed class StockReservedDomainEvent : DomainEventBase
{
    public Guid ReservationId { get; }
    public Guid ProductId { get; }
    public string ReferenceType { get; }
    public Guid ReferenceId { get; }
    public int Quantity { get; }
    public DateTime ExpiresAt { get; }

    public override string AggregateType => "StockReservation";
    public override Guid AggregateId => ReservationId;
    public override string Module => "catalog";

    public StockReservedDomainEvent(Guid reservationId, Guid productId, string referenceType, Guid referenceId, int quantity, DateTime expiresAt)
    {
        ReservationId = reservationId;
        ProductId = productId;
        ReferenceType = referenceType;
        ReferenceId = referenceId;
        Quantity = quantity;
        ExpiresAt = expiresAt;
    }
}



