namespace BuildingBlocks.Web;

/// <summary>
/// Resposta paginada da API.
/// </summary>
/// <typeparam name="T">Tipo dos itens.</typeparam>
public sealed class PaginatedResponse<T>
{
    /// <summary>
    /// Indica que a operação foi bem-sucedida.
    /// </summary>
    public bool Success { get; init; } = true;

    /// <summary>
    /// Itens da página atual.
    /// </summary>
    public required IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// Metadados de paginação.
    /// </summary>
    public required PaginationMeta Pagination { get; init; }

    /// <summary>
    /// Timestamp da resposta (UTC).
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Cria resposta paginada.
    /// </summary>
    public static PaginatedResponse<T> Create(
        IReadOnlyList<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        return new PaginatedResponse<T>
        {
            Items = items,
            Pagination = new PaginationMeta
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
    }

    /// <summary>
    /// Cria resposta paginada a partir de PagedResult.
    /// </summary>
    public static PaginatedResponse<T> FromPagedResult(
        BuildingBlocks.Application.Models.PagedResult<T> pagedResult)
    {
        return new PaginatedResponse<T>
        {
            Items = pagedResult.Items,
            Pagination = new PaginationMeta
            {
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                HasPreviousPage = pagedResult.HasPreviousPage,
                HasNextPage = pagedResult.HasNextPage
            }
        };
    }
}

/// <summary>
/// Metadados de paginação.
/// </summary>
public sealed record PaginationMeta
{
    /// <summary>
    /// Número da página atual (1-based).
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total de itens em todas as páginas.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Total de páginas.
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Indica se há página anterior.
    /// </summary>
    public bool HasPreviousPage { get; init; }

    /// <summary>
    /// Indica se há próxima página.
    /// </summary>
    public bool HasNextPage { get; init; }
}
