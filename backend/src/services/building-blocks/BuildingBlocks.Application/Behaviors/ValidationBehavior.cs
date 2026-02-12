using BuildingBlocks.Application.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;


/// <summary>
/// Behavior do pipeline MediatR que executa validação automática.
/// Utiliza FluentValidation para validar requests antes de chegarem ao handler.
/// </summary>
/// <typeparam name="TRequest">Tipo do request.</typeparam>
/// <typeparam name="TResponse">Tipo da resposta.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Se não há validators, continua para o próximo handler
        if (!_validators.Any())
        {
            return await next();
        }

        var requestName = typeof(TRequest).Name;
        _logger.LogDebug("Validando request {RequestName}", requestName);

        // Executa todos os validators
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Coleta todos os erros
        var failures = validationResults
            .Where(r => r.Errors.Count > 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count > 0)
        {
            _logger.LogWarning(
                "Validação falhou para {RequestName}. {ErrorCount} erros encontrados.",
                requestName,
                failures.Count);

            // Retorna o primeiro erro como Result.Failure
            var firstError = failures.First();
            var error = new Error(
                $"Validation.{firstError.PropertyName}",
                firstError.ErrorMessage);

            // Cria instância de Result com falha
            // Se TResponse é Result<T>, precisamos criar Result<T>.Failure
            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result)
                    .GetMethod(nameof(Result.Failure), 1, new[] { typeof(Error) })!
                    .MakeGenericMethod(valueType);

                return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
            }

            return (TResponse)(object)Result.Failure(error);
        }

        _logger.LogDebug("Validação bem-sucedida para {RequestName}", requestName);
        return await next();
    }
}

/// <summary>
/// Erros de validação agregados.
/// </summary>
public sealed record ValidationErrors
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationErrors(IEnumerable<ValidationError> errors)
    {
        Errors = errors.ToList().AsReadOnly();
    }
}

/// <summary>
/// Representa um erro de validação individual.
/// </summary>
public sealed record ValidationError(string PropertyName, string ErrorMessage);
