
using Catalog.Domain.Aggregates.StockReservation.Events;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.Aggregates.StockReservation;

/// <summary>
/// Aggregate Root representando uma reserva de estoque.
/// Reservas são criadas quando itens são adicionados ao carrinho ou pedidos são iniciados.
/// </summary>
public sealed class StockReservation : AggregateRoot<Guid>
{
    /// <summary>
    /// ID do produto reservado.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Tipo de referência (Cart, Order, etc.).
    /// </summary>
    public string ReferenceType { get; private set; } = string.Empty;

    /// <summary>
    /// ID da referência (ID do carrinho, pedido, etc.).
    /// </summary>
    public Guid ReferenceId { get; private set; }

    /// <summary>
    /// Quantidade reservada.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Data/hora de expiração da reserva.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Data/hora de liberação da reserva (null se ainda ativa).
    /// </summary>
    public DateTime? ReleasedAt { get; private set; }

    /// <summary>
    /// Indica se a reserva está ativa (não expirada e não liberada).
    /// </summary>
    public bool IsActive => ReleasedAt == null && ExpiresAt > DateTime.UtcNow;

    /// <summary>
    /// Indica se a reserva expirou.
    /// </summary>
    public bool IsExpired => ReleasedAt == null && ExpiresAt <= DateTime.UtcNow;

    /// <summary>
    /// Indica se a reserva foi liberada.
    /// </summary>
    public bool IsReleased => ReleasedAt != null;

    /// <summary>
    /// Construtor privado para EF Core.
    /// </summary>
    private StockReservation() : base() { }

    /// <summary>
    /// Construtor privado para criação da reserva.
    /// </summary>
    private StockReservation(
        Guid id,
        Guid productId,
        string referenceType,
        Guid referenceId,
        int quantity,
        DateTime expiresAt) : base(id)
    {
        ProductId = productId;
        ReferenceType = referenceType;
        ReferenceId = referenceId;
        Quantity = quantity;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Factory method para criar uma nova reserva de estoque.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="referenceType">Tipo de referência (Cart, Order).</param>
    /// <param name="referenceId">ID da referência.</param>
    /// <param name="quantity">Quantidade a reservar.</param>
    /// <param name="expirationMinutes">Tempo de expiração em minutos (padrão: 30).</param>
    /// <returns>Nova instância de StockReservation.</returns>
    public static StockReservation Create(
        Guid productId,
        string referenceType,
        Guid referenceId,
        int quantity,
        int expirationMinutes = 30)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade a reservar deve ser maior que zero.", nameof(quantity));

        if (string.IsNullOrWhiteSpace(referenceType))
            throw new ArgumentException("O tipo de referência é obrigatório.", nameof(referenceType));

        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var reservation = new StockReservation(
            Guid.NewGuid(),
            productId,
            referenceType,
            referenceId,
            quantity,
            expiresAt);

        reservation.RegisterDomainEvent(new StockReservedDomainEvent(
            reservation.Id,
            reservation.ProductId,
            reservation.ReferenceType,
            reservation.ReferenceId,
            reservation.Quantity,
            reservation.ExpiresAt));

        return reservation;
    }

    /// <summary>
    /// Atualiza a quantidade reservada.
    /// </summary>
    /// <param name="newQuantity">Nova quantidade.</param>
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(newQuantity));

        if (IsReleased)
            throw new InvalidOperationException("Não é possível alterar uma reserva já liberada.");

        Quantity = newQuantity;
        IncrementVersion();
    }

    /// <summary>
    /// Estende a data de expiração da reserva.
    /// </summary>
    /// <param name="additionalMinutes">Minutos adicionais.</param>
    public void Extend(int additionalMinutes)
    {
        if (IsReleased)
            throw new InvalidOperationException("Não é possível estender uma reserva já liberada.");

        ExpiresAt = DateTime.UtcNow.AddMinutes(additionalMinutes);
        IncrementVersion();
    }

    /// <summary>
    /// Libera a reserva (cancela ou confirma a venda).
    /// </summary>
    /// <param name="reason">Motivo da liberação.</param>
    public void Release(string? reason = null)
    {
        if (IsReleased)
            return;

        ReleasedAt = DateTime.UtcNow;
        IncrementVersion();

        RegisterDomainEvent(new StockReleasedDomainEvent(
            Id,
            ProductId,
            ReferenceType,
            ReferenceId,
            Quantity,
            reason));
    }

    /// <summary>
    /// Marca a reserva como expirada e libera.
    /// </summary>
    public void MarkAsExpired()
    {
        if (IsReleased)
            return;

        ReleasedAt = DateTime.UtcNow;
        IncrementVersion();

        RegisterDomainEvent(new StockReleasedDomainEvent(
            Id,
            ProductId,
            ReferenceType,
            ReferenceId,
            Quantity,
            "Reserva expirada"));
    }
}
