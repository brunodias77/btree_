using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Interface para entidades que possuem eventos de domínio.
/// Permite que interceptors e outros componentes acessem os eventos.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Coleção somente leitura de eventos de domínio pendentes.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Remove todos os eventos de domínio pendentes.
    /// </summary>
    void ClearDomainEvents();
}