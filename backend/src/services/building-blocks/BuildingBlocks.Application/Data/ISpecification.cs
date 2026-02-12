using System.Linq.Expressions;

namespace BuildingBlocks.Application.Data;


/// <summary>
/// Interface para o padrão Specification.
/// Encapsula critérios de consulta reutilizáveis para queries complexas.
/// Mapeado para views complexas do schema (ex: v_active_coupons).
/// </summary>
/// <typeparam name="T">Tipo da entidade.</typeparam>
public interface ISpecification<T> where T : class
{
    /// <summary>
    /// Expressão de filtro (WHERE).
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Lista de includes para carregamento eager (expressões).
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Lista de includes para carregamento eager (strings para includes aninhados).
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Expressão de ordenação crescente.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Expressão de ordenação decrescente.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Expressão de agrupamento.
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }

    /// <summary>
    /// Número de registros a pular (paginação).
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Número máximo de registros a retornar (paginação).
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Indica se a paginação está habilitada.
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    /// Indica se deve usar AsNoTracking para leitura.
    /// </summary>
    bool AsNoTracking { get; }

    /// <summary>
    /// Indica se deve usar AsSplitQuery para múltiplos includes.
    /// </summary>
    bool AsSplitQuery { get; }
}
