namespace BuildingBlocks.Application.Models;
/// <summary>
/// Representa um resultado paginado.
/// </summary>
/// <typeparam name="T">Tipo dos itens.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Itens da página atual.
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Número da página atual (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Total de itens em todas as páginas.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Total de páginas.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indica se há página anterior.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indica se há próxima página.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResult(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Cria um resultado paginado vazio.
    /// </summary>
    public static PagedResult<T> Empty(int pageSize = 10) =>
        new(Array.Empty<T>(), 1, pageSize, 0);

    /// <summary>
    /// Cria um resultado paginado a partir de uma lista.
    /// </summary>
    public static PagedResult<T> Create(
        IEnumerable<T> source,
        int pageNumber,
        int pageSize)
    {
        var items = source.ToList();
        var totalCount = items.Count;
        var pagedItems = items
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>(pagedItems, pageNumber, pageSize, totalCount);
    }

    /// <summary>
    /// Mapeia os itens para outro tipo.
    /// </summary>
    public PagedResult<TOut> Map<TOut>(Func<T, TOut> mapper)
    {
        var mappedItems = Items.Select(mapper).ToList();
        return new PagedResult<TOut>(mappedItems, PageNumber, PageSize, TotalCount);
    }
}

/// <summary>
/// Request de paginação.
/// </summary>
public record PagedRequest
{
    /// <summary>
    /// Número da página (1-based). Padrão: 1.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Tamanho da página. Padrão: 10, Máximo: 100.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Campo para ordenação.
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Direção da ordenação (asc/desc).
    /// </summary>
    public string? SortDirection { get; init; }

    /// <summary>
    /// Indica se ordenação é descendente.
    /// </summary>
    public bool IsDescending => 
        string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Calcula o número de registros a pular.
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Validação e normalização dos valores.
    /// </summary>
    public PagedRequest Normalize()
    {
        return this with
        {
            PageNumber = Math.Max(1, PageNumber),
            PageSize = Math.Clamp(PageSize, 1, 100)
        };
    }
}
