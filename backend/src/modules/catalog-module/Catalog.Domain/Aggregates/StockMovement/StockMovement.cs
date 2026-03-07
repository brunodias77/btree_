using Catalog.Domain.Aggregates.StockMovement.Events;
using Catalog.Domain.Enums;
using Shared.Domain.Abstractions;

namespace Catalog.Domain.Aggregates.StockMovement;

/// <summary>
/// Aggregate Root representando uma movimentação de estoque.
/// Registra histórico de entradas, saídas, ajustes e reservas de estoque.
/// </summary>
public sealed class StockMovement : AggregateRoot<Guid>
{
    /// <summary>
    /// ID do produto relacionado à movimentação.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Tipo de movimentação (entrada, saída, ajuste, etc.).
    /// </summary>
    public StockMovementType MovementType { get; private set; }

    /// <summary>
    /// Quantidade movimentada (positivo para entrada, negativo para saída).
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Tipo de referência (Cart, Order, Manual, etc.).
    /// </summary>
    public string? ReferenceType { get; private set; }

    /// <summary>
    /// ID da referência (ID do carrinho, pedido, etc.).
    /// </summary>
    public Guid? ReferenceId { get; private set; }

    /// <summary>
    /// Estoque antes da movimentação.
    /// </summary>
    public int StockBefore { get; private set; }

    /// <summary>
    /// Estoque após a movimentação.
    /// </summary>
    public int StockAfter { get; private set; }

    /// <summary>
    /// Motivo/justificativa da movimentação.
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// ID do usuário que realizou a movimentação.
    /// </summary>
    public Guid? PerformedBy { get; private set; }

    /// <summary>
    /// Construtor privado para EF Core.
    /// </summary>
    private StockMovement() : base() { }

    /// <summary>
    /// Construtor privado para criação da movimentação.
    /// </summary>
    private StockMovement(
        Guid id,
        Guid productId,
        StockMovementType movementType,
        int quantity,
        int stockBefore,
        int stockAfter) : base(id)
    {
        ProductId = productId;
        MovementType = movementType;
        Quantity = quantity;
        StockBefore = stockBefore;
        StockAfter = stockAfter;
    }

    /// <summary>
    /// Factory method para criar uma movimentação de entrada de estoque.
    /// </summary>
    public static StockMovement CreateInbound(
        Guid productId,
        int quantity,
        int stockBefore,
        string? referenceType = null,
        Guid? referenceId = null,
        string? reason = null,
        Guid? performedBy = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade de entrada deve ser maior que zero.", nameof(quantity));

        var stockAfter = stockBefore + quantity;
        var movement = new StockMovement(
            Guid.NewGuid(),
            productId,
            StockMovementType.In,
            quantity,
            stockBefore,
            stockAfter)
        {
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Reason = reason,
            PerformedBy = performedBy
        };

        movement.RegisterDomainEvent(new StockMovementCreatedDomainEvent(
            movement.Id,
            movement.ProductId,
            movement.MovementType,
            movement.Quantity,
            movement.StockBefore,
            movement.StockAfter));

        return movement;
    }

    /// <summary>
    /// Factory method para criar uma movimentação de saída de estoque.
    /// </summary>
    public static StockMovement CreateOutbound(
        Guid productId,
        int quantity,
        int stockBefore,
        string? referenceType = null,
        Guid? referenceId = null,
        string? reason = null,
        Guid? performedBy = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade de saída deve ser maior que zero.", nameof(quantity));

        if (stockBefore < quantity)
            throw new InvalidOperationException("Estoque insuficiente para a saída.");

        var stockAfter = stockBefore - quantity;
        var movement = new StockMovement(
            Guid.NewGuid(),
            productId,
            StockMovementType.Out,
            -quantity,
            stockBefore,
            stockAfter)
        {
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Reason = reason,
            PerformedBy = performedBy
        };

        movement.RegisterDomainEvent(new StockMovementCreatedDomainEvent(
            movement.Id,
            movement.ProductId,
            movement.MovementType,
            movement.Quantity,
            movement.StockBefore,
            movement.StockAfter));

        return movement;
    }

    /// <summary>
    /// Factory method para criar um ajuste de estoque.
    /// </summary>
    public static StockMovement CreateAdjustment(
        Guid productId,
        int newStock,
        int stockBefore,
        string? reason = null,
        Guid? performedBy = null)
    {
        if (newStock < 0)
            throw new ArgumentException("O estoque não pode ser negativo.", nameof(newStock));

        var quantity = newStock - stockBefore;
        var movement = new StockMovement(
            Guid.NewGuid(),
            productId,
            StockMovementType.Adjustment,
            quantity,
            stockBefore,
            newStock)
        {
            ReferenceType = "Manual",
            Reason = reason,
            PerformedBy = performedBy
        };

        movement.RegisterDomainEvent(new StockMovementCreatedDomainEvent(
            movement.Id,
            movement.ProductId,
            movement.MovementType,
            movement.Quantity,
            movement.StockBefore,
            movement.StockAfter));

        return movement;
    }

    /// <summary>
    /// Factory method para criar uma movimentação de reserva.
    /// </summary>
    public static StockMovement CreateReserve(
        Guid productId,
        int quantity,
        int stockBefore,
        string referenceType,
        Guid referenceId)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade a reservar deve ser maior que zero.", nameof(quantity));

        var movement = new StockMovement(
            Guid.NewGuid(),
            productId,
            StockMovementType.Reserve,
            -quantity,
            stockBefore,
            stockBefore) // Reserva não altera estoque físico
        {
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Reason = $"Reserva para {referenceType}"
        };

        movement.RegisterDomainEvent(new StockMovementCreatedDomainEvent(
            movement.Id,
            movement.ProductId,
            movement.MovementType,
            movement.Quantity,
            movement.StockBefore,
            movement.StockAfter));

        return movement;
    }

    /// <summary>
    /// Factory method para criar uma movimentação de liberação de reserva.
    /// </summary>
    public static StockMovement CreateRelease(
        Guid productId,
        int quantity,
        int stockBefore,
        string referenceType,
        Guid referenceId,
        string? reason = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade a liberar deve ser maior que zero.", nameof(quantity));

        var movement = new StockMovement(
            Guid.NewGuid(),
            productId,
            StockMovementType.Release,
            quantity,
            stockBefore,
            stockBefore) // Liberação não altera estoque físico
        {
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Reason = reason ?? $"Liberação de reserva de {referenceType}"
        };

        movement.RegisterDomainEvent(new StockMovementCreatedDomainEvent(
            movement.Id,
            movement.ProductId,
            movement.MovementType,
            movement.Quantity,
            movement.StockBefore,
            movement.StockAfter));

        return movement;
    }
}
