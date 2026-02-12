using BuildingBlocks.Web.Filters;

namespace BuildingBlocks.Web;

/// <summary>
/// Resposta de erro da API.
/// </summary>
public sealed class ApiErrorResponse
{
    /// <summary>
    /// Indica que a operação falhou.
    /// </summary>
    public bool Success { get; init; } = false;

    /// <summary>
    /// Detalhes do erro.
    /// </summary>
    public required ErrorDetails Error { get; init; }

    /// <summary>
    /// Erros de validação (quando aplicável).
    /// </summary>
    public IReadOnlyList<ValidationError>? ValidationErrors { get; init; }

    /// <summary>
    /// ID de correlação para rastreamento.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Timestamp da resposta (UTC).
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    #region Factory Methods

    /// <summary>
    /// Cria resposta de erro genérica.
    /// </summary>
    public static ApiErrorResponse BadRequest(string code, string message, string? correlationId = null) => new()
    {
        Error = new ErrorDetails { Code = code, Message = message },
        CorrelationId = correlationId
    };

    /// <summary>
    /// Cria resposta de recurso não encontrado.
    /// </summary>
    public static ApiErrorResponse NotFound(string message, string? correlationId = null) => new()
    {
        Error = new ErrorDetails { Code = "NotFound", Message = message },
        CorrelationId = correlationId
    };

    /// <summary>
    /// Cria resposta de conflito.
    /// </summary>
    public static ApiErrorResponse Conflict(string message, string? correlationId = null) => new()
    {
        Error = new ErrorDetails { Code = "Conflict", Message = message },
        CorrelationId = correlationId
    };

    /// <summary>
    /// Cria resposta de erro de validação simples.
    /// </summary>
    public static ApiErrorResponse ValidationError(string message, string? correlationId = null) => new()
    {
        Error = new ErrorDetails { Code = "ValidationError", Message = message },
        CorrelationId = correlationId
    };

    /// <summary>
    /// Cria resposta de erros de validação múltiplos.
    /// </summary>
    public static ApiErrorResponse WithValidationErrors(
        IReadOnlyList<ValidationError> errors,
        string? correlationId = null) => new()
    {
        Error = new ErrorDetails { Code = "ValidationError", Message = "Um ou mais erros de validação ocorreram." },
        ValidationErrors = errors,
        CorrelationId = correlationId
    };

    /// <summary>
    /// Cria resposta de não autorizado.
    /// </summary>
    public static ApiErrorResponse Unauthorized(string message, string? correlationId = null) => new()
    {
        Error = new ErrorDetails { Code = "Unauthorized", Message = message },
        CorrelationId = correlationId
    };

    /// <summary>
    /// Cria resposta de acesso negado.
    /// </summary>
    public static ApiErrorResponse Forbidden(string message, string? correlationId = null) => new()
    {
        Error = new ErrorDetails { Code = "Forbidden", Message = message },
        CorrelationId = correlationId
    };

    /// <summary>
    /// Cria resposta de erro interno.
    /// </summary>
    public static ApiErrorResponse InternalError(string? correlationId = null) => new()
    {
        Error = new ErrorDetails
        {
            Code = "InternalError",
            Message = "Ocorreu um erro interno. Por favor, tente novamente mais tarde."
        },
        CorrelationId = correlationId
    };

    #endregion
}

/// <summary>
/// Detalhes do erro.
/// </summary>
public sealed record ErrorDetails
{
    /// <summary>
    /// Código do erro para identificação programática.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Mensagem descritiva do erro.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Detalhes adicionais (stack trace em dev, etc.).
    /// </summary>
    public string? Details { get; init; }
}
