using MediatR;

namespace BuildingBlocks.Domain.Events;

/// <summary>
/// Interface para handlers de eventos de domínio.
/// Handlers processam eventos de domínio e executam side effects ou atualizam outros agregados.
/// </summary>
/// <typeparam name="TEvent">O tipo do evento de domínio a ser tratado.</typeparam>
public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}