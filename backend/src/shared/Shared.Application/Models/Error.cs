namespace Shared.Application.Models;

/// <summary>
/// Representa um erro de aplicação.
/// </summary>
/// <param name="Code">Código único do erro para identificação programática.</param>
/// <param name="Message">Mensagem descritiva do erro.</param>
public sealed record Error(string Code, string Message)
{
    /// <summary>
    /// Erro nulo/vazio, representa ausência de erro.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Erro genérico de operação nula.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "O valor fornecido é nulo.");

    /// <summary>
    /// Erro de item não encontrado.
    /// </summary>
    public static Error NotFound(string entity, object id) =>
        new($"{entity}.NotFound", $"{entity} com ID '{id}' não foi encontrado.");

    /// <summary>
    /// Erro de item não encontrado com código e mensagem customizados.
    /// </summary>
    public static Error NotFound(string code, string message) =>
        new(code, message);

    /// <summary>
    /// Erro de conflito/duplicação.
    /// </summary>
    public static Error Conflict(string entity, string reason) =>
        new($"{entity}.Conflict", reason);

    /// <summary>
    /// Erro de validação.
    /// </summary>
    public static Error Validation(string field, string message) =>
        new($"Validation.{field}", message);

    /// <summary>
    /// Erro de falha genérica.
    /// </summary>
    public static Error Failure(string code, string message) =>
        new(code, message);

    /// <summary>
    /// Erro de não autorizado.
    /// </summary>
    public static Error Unauthorized(string code = "Auth.Unauthorized", string message = "Não autorizado.") =>
        new(code, message);

    /// <summary>
    /// Erro de acesso negado.
    /// </summary>
    public static Error Forbidden(string code = "Auth.Forbidden", string message = "Acesso negado.") =>
        new(code, message);
}