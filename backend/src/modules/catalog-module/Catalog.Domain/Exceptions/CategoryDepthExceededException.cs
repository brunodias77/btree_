using Shared.Domain.Exceptions;

namespace Catalog.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando a profundidade máxima de categorias é excedida.
/// </summary>
public class CategoryDepthExceededException : DomainException
{
    public int MaxDepth { get; }
    public int AttemptedDepth { get; }

    public CategoryDepthExceededException(int maxDepth, int attemptedDepth)
        : base("CATEGORY_DEPTH_EXCEEDED", 
            $"A profundidade máxima de {maxDepth} níveis para categorias foi excedida. Tentativa de criar categoria no nível {attemptedDepth}.")
    {
        MaxDepth = maxDepth;
        AttemptedDepth = attemptedDepth;
    }

    public CategoryDepthExceededException(int maxDepth)
        : base("CATEGORY_DEPTH_EXCEEDED", 
            $"A profundidade máxima de {maxDepth} níveis para categorias foi excedida.")
    {
        MaxDepth = maxDepth;
        AttemptedDepth = maxDepth + 1;
    }
}
