using Microsoft.Extensions.Logging;
using Shared.Application.Events.Handlers;
using Users.Domain.Aggregates.Profiles.Events;

namespace Users.Application.Events.Profile;

public class ProfileCreatedDomainEventHandler : IDomainEventHandler<ProfileCreatedDomainEvent>
{
    private readonly ILogger<ProfileCreatedDomainEventHandler> _logger;

    public ProfileCreatedDomainEventHandler(ILogger<ProfileCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ProfileCreatedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Domain Event Handled: {EventName} - ProfileId: {ProfileId}, UserId: {UserId}, Email: {Email}, Name: {Name}", 
            nameof(ProfileCreatedDomainEvent), 
            @event.AggregateId, 
            @event.UserId, 
            @event.Email, 
            @event.Name);

        // TODO: Enviar email de boas vindas, notificar sistemas terceiros, etc.
        // Lancar o evento de criacao de carrinho no outro modulo

        return Task.CompletedTask;
    }
}