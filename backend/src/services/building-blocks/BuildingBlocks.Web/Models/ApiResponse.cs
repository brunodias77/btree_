namespace BuildingBlocks.Web;

/// <summary>
/// Resposta genérica de sucesso da API.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    public bool Success { get; init; } = true;

    /// <summary>
    /// Mensagem opcional.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Timestamp da resposta (UTC).
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Cria resposta de sucesso.
    /// </summary>
    public static ApiResponse Ok(string? message = null) => new()
    {
        Success = true,
        Message = message
    };
}

/// <summary>
/// Resposta genérica de sucesso com dados.
/// </summary>
/// <typeparam name="T">Tipo dos dados.</typeparam>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Dados da resposta.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Cria resposta de sucesso com dados.
    /// </summary>
    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };
}
