using BuildingBlocks.Shared.IntegrationEvents.Users;
//using Carts.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Carts.Application.EventHandlers.Users;

public sealed class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🚀 [CartsModule] Recebido evento UserRegisteredEvent para usuário {UserId}. Preparando para criar carrinho...",
            notification.UserId);
            
        return Task.CompletedTask;
    }

}