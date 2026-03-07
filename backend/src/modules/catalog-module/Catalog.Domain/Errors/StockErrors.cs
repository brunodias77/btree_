
using Shared.Application.Models;

namespace Catalog.Domain.Errors;

/// <summary>
/// Erros relacionados a estoque.
/// </summary>
public static class StockErrors
{
    public static Error InsufficientStock(Guid productId, int requested, int available) =>
        Error.Failure("Stock.InsufficientStock", 
            $"Estoque insuficiente para o produto {productId}. Solicitado: {requested}, Disponível: {available}.");

    public static Error InsufficientStockBySku(string sku, int requested, int available) =>
        Error.Failure("Stock.InsufficientStock", 
            $"Estoque insuficiente para o produto '{sku}'. Solicitado: {requested}, Disponível: {available}.");

    public static Error ReservationNotFound(Guid reservationId) =>
        Error.NotFound("StockReservation", reservationId);

    public static Error ReservationExpired(Guid reservationId) =>
        Error.Failure("Stock.ReservationExpired", 
            $"A reserva de estoque {reservationId} expirou.");

    public static Error ReservationAlreadyReleased(Guid reservationId) =>
        Error.Failure("Stock.ReservationAlreadyReleased", 
            $"A reserva de estoque {reservationId} já foi liberada.");

    public static Error InvalidQuantity =>
        Error.Validation("Stock.Quantity", "A quantidade deve ser maior que zero.");

    public static Error NegativeStock =>
        Error.Validation("Stock.NegativeStock", "O estoque não pode ser negativo.");

    public static Error ReservedStockExceeded =>
        Error.Failure("Stock.ReservedStockExceeded", 
            "A quantidade a liberar é maior que o estoque reservado.");
}
