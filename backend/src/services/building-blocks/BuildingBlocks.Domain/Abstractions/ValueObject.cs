namespace BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Classe base para value objects.
/// Value objects são imutáveis e definidos por seus valores, não por identidade.
/// Dois value objects são iguais se todos os seus componentes forem iguais.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Retorna os componentes que definem a igualdade deste value object.
    /// </summary>
    /// <returns>Enumeração de componentes de igualdade.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;
        return Equals((ValueObject)obj);
    }

    public bool Equals(ValueObject? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Where(c => c is not null)
            .Aggregate(17, (current, component) => current * 23 + component!.GetHashCode());
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Cria uma cópia do value object.
    /// Como value objects são imutáveis, retorna a própria instância.
    /// </summary>
    public ValueObject Copy()
    {
        return (ValueObject)MemberwiseClone();
    }
}
