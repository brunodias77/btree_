namespace BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Entidades são definidas por sua identidade e possuem um ciclo de vida.
/// Implementa soft delete com DeletedAt e optimistic locking com Version.
/// </summary>
/// <typeparam name="TId">O tipo do identificador da entidade.</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>
    /// Identificador único da entidade.
    /// </summary>
    public TId Id { get; protected set; }

    /// <summary>
    /// Data/hora de criação (UTC).
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Data/hora da última atualização (UTC).
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// Data/hora da exclusão lógica (UTC).
    /// Null se a entidade não foi excluída.
    /// </summary>
    public DateTime? DeletedAt { get; protected set; }

    /// <summary>
    /// Indica se a entidade foi excluída logicamente.
    /// </summary>
    public bool IsDeleted => DeletedAt.HasValue;

    protected Entity(TId id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Construtor protegido para EF Core.
    /// </summary>
    protected Entity()
    {
        Id = default!;
    }

    /// <summary>
    /// Marca a entidade como excluída (soft delete).
    /// </summary>
    public virtual void Delete()
    {
        if (!IsDeleted)
        {
            DeletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Restaura uma entidade excluída.
    /// </summary>
    public virtual void Restore()
    {
        DeletedAt = null;
    }

    /// <summary>
    /// Atualiza o timestamp de modificação.
    /// </summary>
    protected void MarkAsModified()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;
        if (obj is not Entity<TId> entity) return false;
        return Equals(entity);
    }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;
        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() * 41;
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }
}
