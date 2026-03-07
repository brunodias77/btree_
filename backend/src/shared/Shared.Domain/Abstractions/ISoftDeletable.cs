namespace Shared.Domain.Abstractions;

/// <summary>
/// Interface para entidades que suportam soft delete.
/// Corresponde às tabelas do SQL que têm coluna deleted_at e partial indexes WHERE deleted_at IS NULL.
/// </summary>
public interface ISoftDeletable
{
    DateTime? DeletedAt { get; }
    void MarkAsDeleted();
}
