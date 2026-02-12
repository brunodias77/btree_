namespace BuildingBlocks.Domain.Abstractions;

using BuildingBlocks.Domain.Events;


/// <summary>
/// Classe base para aggregate roots.
/// Aggregate roots são pontos de entrada para todas as operações dentro de um agregado.
/// Mantêm limites de consistência, controlam eventos de domínio e implementam optimistic locking.
/// </summary>
/// <typeparam name="TId">O tipo do identificador do aggregate root.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IHasDomainEvents
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Versão atual do agregado para controle de concorrência otimista.
    /// Mapeado para coluna 'version INT' no banco de dados.
    /// </summary>
    public int Version { get; private set; } = 1;

    /// <summary>
    /// Coleção somente leitura de eventos de domínio pendentes.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Construtor protegido para EF Core.
    /// </summary>
    protected AggregateRoot()
    {
    }

    /// <summary>
    /// Registra um evento de domínio para ser despachado.
    /// </summary>
    /// <param name="domainEvent">O evento de domínio a ser registrado.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    /// <summary>
    /// Remove todos os eventos de domínio pendentes.
    /// Geralmente chamado após o despacho dos eventos.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Incrementa a versão do agregado.
    /// Usado para controle de concorrência otimista (optimistic locking).
    /// </summary>
    public void IncrementVersion()
    {
        Version++;
        MarkAsModified();
    }

    /// <summary>
    /// Define a versão manualmente (útil para reconstrução a partir do banco).
    /// </summary>
    /// <param name="version">Nova versão.</param>
    protected void SetVersion(int version)
    {
        Version = version;
    }
}
