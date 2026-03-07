using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Web.Models;

/// <summary>
/// Resposta padronizada de erro da API.
/// </summary>
public class ApiErrorResponse
{
    public bool Success { get; init; } = false;

    /// <summary>
    /// Código de erro (ex: "NotFound", "Validation").
    /// </summary>
    public required string Error { get; init; }

    /// <summary>
    /// Mensagem legível do erro.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Erros de validação por campo (opcional).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; init; }

    /// <summary>
    /// ID de correlação para rastreamento (opcional).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CorrelationId { get; init; }

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    // ── Factory Methods ──

    public static ApiErrorResponse BadRequest(string code, string message) => new()
    {
        Error = code,
        Message = message
    };

    public static ApiErrorResponse ValidationError(string message, IDictionary<string, string[]>? errors = null) => new()
    {
        Error = "Validation",
        Message = message,
        Errors = errors
    };

    public static ApiErrorResponse NotFound(string message) => new()
    {
        Error = "NotFound",
        Message = message
    };

    public static ApiErrorResponse Conflict(string message) => new()
    {
        Error = "Conflict",
        Message = message
    };

    public static ApiErrorResponse Unauthorized(string message = "Não autorizado.") => new()
    {
        Error = "Unauthorized",
        Message = message
    };

    public static ApiErrorResponse Forbidden(string message = "Acesso negado.") => new()
    {
        Error = "Forbidden",
        Message = message
    };

    public static ApiErrorResponse InternalError(string message = "Ocorreu um erro interno no servidor.") => new()
    {
        Error = "InternalError",
        Message = message
    };

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
