using Shared.Domain.Abstractions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Value Object representando o caminho hierárquico de uma categoria.
/// </summary>
public sealed class CategoryPath : ValueObject
{
    /// <summary>
    /// Separador de níveis do caminho.
    /// </summary>
    public const string Separator = "/";

    /// <summary>
    /// Profundidade máxima permitida.
    /// </summary>
    public const int MaxDepth = 5;

    /// <summary>
    /// Valor do caminho (ex: "/eletronicos/celulares/smartphones").
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Profundidade do caminho (número de níveis).
    /// </summary>
    public int Depth { get; }

    /// <summary>
    /// Segmentos do caminho.
    /// </summary>
    public IReadOnlyList<string> Segments { get; }

    private CategoryPath(string value, int depth, IReadOnlyList<string> segments)
    {
        Value = value;
        Depth = depth;
        Segments = segments;
    }

    /// <summary>
    /// Cria um novo caminho de categoria.
    /// </summary>
    /// <param name="path">Caminho completo.</param>
    /// <returns>Nova instância de CategoryPath.</returns>
    public static CategoryPath Create(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("O caminho não pode ser vazio.", nameof(path));

        var normalized = path.Trim().ToLowerInvariant();
        
        if (!normalized.StartsWith(Separator))
            normalized = Separator + normalized;

        var segments = normalized
            .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
            .ToList()
            .AsReadOnly();

        var depth = segments.Count;

        if (depth > MaxDepth)
            throw new ArgumentException($"A profundidade máxima ({MaxDepth}) foi excedida.", nameof(path));

        return new CategoryPath(normalized, depth, segments);
    }

    /// <summary>
    /// Cria caminho raiz.
    /// </summary>
    public static CategoryPath Root(string slug) => Create($"/{slug}");

    /// <summary>
    /// Cria caminho filho.
    /// </summary>
    public CategoryPath Append(string childSlug)
    {
        if (Depth >= MaxDepth)
            throw new InvalidOperationException($"A profundidade máxima ({MaxDepth}) foi atingida.");

        return Create($"{Value}/{childSlug}");
    }

    /// <summary>
    /// Retorna o caminho do pai (um nível acima).
    /// </summary>
    public CategoryPath? Parent
    {
        get
        {
            if (Depth <= 1)
                return null;

            var parentPath = string.Join(Separator, Segments.Take(Depth - 1));
            return Create($"/{parentPath}");
        }
    }

    /// <summary>
    /// Verifica se este caminho contém outro (é ancestral).
    /// </summary>
    public bool Contains(CategoryPath other)
    {
        return other.Value.StartsWith(Value + Separator) || other.Value == Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(CategoryPath path) => path.Value;
}
