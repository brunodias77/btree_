namespace BuildingBlocks.Application.Idempotency;

/// <summary>
/// Interface marcadora para requests idempotentes.
/// Usado especialmente em operações de pagamento onde retry pode causar duplicação.
/// Mapeado para coluna 'idempotency_key' no schema de payments.
/// </summary>
public interface IIdempotentRequest
{
    /// <summary>
    /// Chave de idempotência única para este request.
    /// Gerada pelo cliente e enviada no header X-Idempotency-Key.
    /// </summary>
    Guid IdempotencyKey { get; }
}