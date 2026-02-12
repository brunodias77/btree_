using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Idempotency;

/// <summary>
/// Behavior do pipeline MediatR que garante idempotência de requests.
/// Verifica se o request já foi processado antes de executar o handler.
/// Crítico para operações financeiras (pagamentos, reembolsos).
/// </summary>
/// <typeparam name="TRequest">Tipo do request.</typeparam>
/// <typeparam name="TResponse">Tipo da resposta.</typeparam>
public sealed class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentRequest
    where TResponse : Result
{
    private readonly IIdempotencyService _idempotencyService;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

    public IdempotencyBehavior(
        IIdempotencyService idempotencyService,
        ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    {
        _idempotencyService = idempotencyService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var idempotencyKey = request.IdempotencyKey;

        // Verifica se já foi processado
        var existingResult = await _idempotencyService.GetResultAsync<TResponse>(
            idempotencyKey,
            cancellationToken);

        if (existingResult is not null)
        {
            _logger.LogInformation(
                "Request com chave de idempotência {IdempotencyKey} já foi processado. Retornando resultado em cache.",
                idempotencyKey);

            return existingResult;
        }

        // Tenta adquirir lock para processamento
        if (!await _idempotencyService.TryAcquireLockAsync(idempotencyKey, cancellationToken))
        {
            _logger.LogWarning(
                "Request com chave de idempotência {IdempotencyKey} já está sendo processado por outro handler.",
                idempotencyKey);

            // Retorna erro indicando processamento em andamento
            return (TResponse)(object)Result.Failure(
                new Error("Idempotency.InProgress", "Este request já está sendo processado."));
        }

        try
        {
            // Executa o handler
            var result = await next();

            // Salva o resultado para futuros requests com mesma chave
            await _idempotencyService.SaveResultAsync(
                idempotencyKey,
                result,
                cancellationToken);

            _logger.LogDebug(
                "Request com chave de idempotência {IdempotencyKey} processado e resultado armazenado.",
                idempotencyKey);

            return result;
        }
        finally
        {
            // Libera o lock
            await _idempotencyService.ReleaseLockAsync(idempotencyKey, cancellationToken);
        }
    }
}

/// <summary>
/// Interface para serviço de idempotência.
/// Gerencia locks e armazenamento de resultados para requests idempotentes.
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
    /// Obtém resultado já processado para a chave de idempotência.
    /// </summary>
    /// <typeparam name="TResult">Tipo do resultado.</typeparam>
    /// <param name="idempotencyKey">Chave de idempotência.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado se existir, null caso contrário.</returns>
    Task<TResult?> GetResultAsync<TResult>(Guid idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva o resultado para a chave de idempotência.
    /// </summary>
    /// <typeparam name="TResult">Tipo do resultado.</typeparam>
    /// <param name="idempotencyKey">Chave de idempotência.</param>
    /// <param name="result">Resultado a ser salvo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task SaveResultAsync<TResult>(Guid idempotencyKey, TResult result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tenta adquirir lock para processamento do request.
    /// </summary>
    /// <param name="idempotencyKey">Chave de idempotência.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se conseguiu o lock.</returns>
    Task<bool> TryAcquireLockAsync(Guid idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Libera o lock de processamento.
    /// </summary>
    /// <param name="idempotencyKey">Chave de idempotência.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task ReleaseLockAsync(Guid idempotencyKey, CancellationToken cancellationToken = default);
}
