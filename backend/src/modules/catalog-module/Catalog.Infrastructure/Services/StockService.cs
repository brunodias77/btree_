using Catalog.Domain.Aggregates.StockMovement;
using Catalog.Domain.Aggregates.StockReservation;
using Catalog.Domain.Enums;
using Catalog.Domain.Repositories;
using Catalog.Domain.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Application.Data;

namespace Catalog.Infrastructure.Services;

public class StockService : IStockService
{
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IStockReservationRepository _stockReservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StockService(
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IStockReservationRepository stockReservationRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _stockReservationRepository = stockReservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> IsAvailableAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null) return false;

        return product.AvailableStock >= quantity;
    }

    public async Task<Guid> ReserveAsync(
        Guid productId, 
        int quantity, 
        string referenceType, 
        Guid referenceId, 
        int expirationMinutes = 30, 
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null) throw new InvalidOperationException("Produto não encontrado.");

        product.ReserveStock(quantity);

        var reservation = StockReservation.Create(
            productId,
            referenceType,
            referenceId,
            quantity,
            expirationMinutes); // Pass int, not DateTime

        await _stockReservationRepository.AddAsync(reservation, cancellationToken);
        
        return reservation.Id;
    }

    public async Task ReleaseAsync(Guid reservationId, string? reason = null, CancellationToken cancellationToken = default)
    {
        var reservation = await _stockReservationRepository.GetByIdAsync(reservationId, cancellationToken);
        if (reservation == null) 
        {
            return; 
        }

        if (reservation.ReleasedAt != null) return; 

        var product = await _productRepository.GetByIdAsync(reservation.ProductId, cancellationToken);
        if (product != null)
        {
            product.ReleaseStock(reservation.Quantity);
        }

        reservation.Release(reason);
        _stockReservationRepository.Update(reservation);
    }

    public async Task ConfirmAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var reservation = await _stockReservationRepository.GetByIdAsync(reservationId, cancellationToken);
        if (reservation == null) throw new InvalidOperationException("Reserva não encontrada.");
        
        var product = await _productRepository.GetByIdAsync(reservation.ProductId, cancellationToken);
        if (product == null) throw new InvalidOperationException("Produto não encontrado.");

        product.ConfirmReservation(reservation.Quantity);
        
        _stockReservationRepository.Delete(reservation);

        // Registrar movimento de saída (Venda)
        var movement = StockMovement.CreateOutbound(
            product.Id,
            reservation.Quantity,
            product.Stock + reservation.Quantity, // StockBefore
            reservation.ReferenceType,
            reservation.ReferenceId,
            "Venda confirmada via reserva");

        await _stockMovementRepository.AddAsync(movement, cancellationToken);
    }

    public async Task AddStockAsync(
        Guid productId, 
        int quantity, 
        string? reason = null, 
        Guid? performedBy = null, 
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null) throw new InvalidOperationException("Produto não encontrado.");

        var stockBefore = product.Stock;
        product.UpdateStock(product.Stock + quantity, reason ?? "Entrada de estoque");
        
        var movement = StockMovement.CreateInbound(
            productId,
            quantity,
            stockBefore,
            null, // RefType
            null, // RefId
            reason ?? "Ajuste manual",
            performedBy);

        await _stockMovementRepository.AddAsync(movement, cancellationToken);
    }

    public async Task RemoveStockAsync(
        Guid productId, 
        int quantity, 
        string? reason = null, 
        Guid? performedBy = null, 
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null) throw new InvalidOperationException("Produto não encontrado.");

        var stockBefore = product.Stock;
        var newStock = product.Stock - quantity;
        if (newStock < 0) throw new InvalidOperationException("Estoque insuficiente para remover.");

        product.UpdateStock(newStock, reason ?? "Saída de estoque");

        var movement = StockMovement.CreateOutbound(
            productId,
            quantity, 
            stockBefore,
            null,
            null,
            reason ?? "Ajuste manual",
            performedBy);

        await _stockMovementRepository.AddAsync(movement, cancellationToken);
    }

    public async Task AdjustStockAsync(
        Guid productId, 
        int newStock, 
        string? reason = null, 
        Guid? performedBy = null, 
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null) throw new InvalidOperationException("Produto não encontrado.");

        var stockBefore = product.Stock;
        var diff = newStock - stockBefore;
        
        if (diff == 0) return;

        product.UpdateStock(newStock, reason ?? "Ajuste de estoque");
        
        var movement = StockMovement.CreateAdjustment(
            productId,
            newStock,
            stockBefore,
            reason ?? "Ajuste absoluto",
            performedBy);

        await _stockMovementRepository.AddAsync(movement, cancellationToken);
    }

    public async Task<int> GetAvailableStockAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        return product?.AvailableStock ?? 0;
    }

    public async Task<int> ReleaseExpiredReservationsAsync(CancellationToken cancellationToken = default)
    {
        var expiredReservations = await _stockReservationRepository.GetExpiredReservationsAsync(DateTime.UtcNow, cancellationToken);
        int count = 0;

        foreach (var reservation in expiredReservations)
        {
            var product = await _productRepository.GetByIdAsync(reservation.ProductId, cancellationToken);
            if (product != null)
            {
                product.ReleaseStock(reservation.Quantity);
                reservation.Release("Expirada");
                _stockReservationRepository.Update(reservation);
                count++;
            }
        }
        return count;
    }
}

