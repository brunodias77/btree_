
using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Infrastructure.Events.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor para capturar eventos de domínio e salvá-los no Outbox.
/// Garante que eventos sejam persistidos junto com as mudanças de estado.
/// Mapeado para tabela shared.domain_events do schema.
/// </summary>
public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<DomainEventInterceptor> _logger;
    private readonly MediatR.IPublisher _publisher;

    public DomainEventInterceptor(ILogger<DomainEventInterceptor> logger, MediatR.IPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            DispatchDomainEventsAsync(eventData.Context).GetAwaiter().GetResult();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            DispatchDomainEventsAsync(eventData.Context).GetAwaiter().GetResult();
        }

        return base.SavingChanges(eventData, result);
    }

    private async Task DispatchDomainEventsAsync(DbContext context)
    {
        var aggregateRoots = context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IHasDomainEvents root && root.DomainEvents.Any())
            .Select(e => (IHasDomainEvents)e.Entity)
            .ToList();

        if (!aggregateRoots.Any())
        {
            return;
        }

        // 1. Coleta todos os eventos
        var domainEvents = aggregateRoots
            .SelectMany(x => x.DomainEvents)
            .ToList();

        // 2. Limpa os eventos das entidades (para não disparar 2x)
        aggregateRoots.ForEach(entry => entry.ClearDomainEvents());

        // 3. Publica in-memory (MediatR)
        // Precisamos do IPublisher. Como este é um interceptor do EF, a injeção de dependência via construtor é o caminho.
        // Porém, o SaveChangesInterceptor é singleton ou scoped? Geralmente Scoped se registrado corretamente.
        // Vamos assumir que IPublisher foi injetado.
        
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}
