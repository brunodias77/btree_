using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Services;

/// <summary>
/// Interface de serviço de domínio para gerenciamento de estoque.
/// </summary>
public interface IStockService
{
    /// <summary>
    /// Verifica se há estoque disponível para uma quantidade.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="quantity">Quantidade desejada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se há estoque suficiente.</returns>
    Task<bool> IsAvailableAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reserva estoque para um carrinho ou pedido.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="quantity">Quantidade a reservar.</param>
    /// <param name="referenceType">Tipo de referência (Cart, Order).</param>
    /// <param name="referenceId">ID da referência.</param>
    /// <param name="expirationMinutes">Tempo de expiração da reserva em minutos.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>ID da reserva criada.</returns>
    Task<Guid> ReserveAsync(
        Guid productId, 
        int quantity, 
        string referenceType, 
        Guid referenceId, 
        int expirationMinutes = 30,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Libera uma reserva de estoque.
    /// </summary>
    /// <param name="reservationId">ID da reserva.</param>
    /// <param name="reason">Motivo da liberação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task ReleaseAsync(Guid reservationId, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma uma reserva (decrementa estoque físico).
    /// </summary>
    /// <param name="reservationId">ID da reserva.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task ConfirmAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona estoque a um produto (entrada).
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="quantity">Quantidade a adicionar.</param>
    /// <param name="reason">Motivo da entrada.</param>
    /// <param name="performedBy">ID do usuário que realizou a operação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task AddStockAsync(
        Guid productId, 
        int quantity, 
        string? reason = null, 
        Guid? performedBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove estoque de um produto (saída).
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="quantity">Quantidade a remover.</param>
    /// <param name="reason">Motivo da saída.</param>
    /// <param name="performedBy">ID do usuário que realizou a operação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task RemoveStockAsync(
        Guid productId, 
        int quantity, 
        string? reason = null, 
        Guid? performedBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ajusta o estoque de um produto para um valor específico.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="newStock">Novo valor de estoque.</param>
    /// <param name="reason">Motivo do ajuste.</param>
    /// <param name="performedBy">ID do usuário que realizou a operação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task AdjustStockAsync(
        Guid productId, 
        int newStock, 
        string? reason = null, 
        Guid? performedBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém o estoque disponível de um produto.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Estoque disponível (total - reservado).</returns>
    Task<int> GetAvailableStockAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Libera reservas expiradas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Número de reservas liberadas.</returns>
    Task<int> ReleaseExpiredReservationsAsync(CancellationToken cancellationToken = default);
}
