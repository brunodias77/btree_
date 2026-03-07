namespace Shared.Domain.Abstractions;

/// <summary>
/// Interface para entidades com controle de versão (optimistic locking).
/// Corresponde às tabelas do SQL com coluna "version INT NOT NULL DEFAULT 1"
/// e trigger trigger_increment_version().
/// </summary>
public interface IVersioned
{
    int Version { get; }
}
