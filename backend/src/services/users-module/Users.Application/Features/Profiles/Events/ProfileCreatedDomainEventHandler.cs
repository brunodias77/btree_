using BuildingBlocks.Application.Data;
using BuildingBlocks.Application.Events;
using MediatR;
using Users.Application.Repositories;
using Users.Domain.Aggregates.Notification;
using Users.Domain.Aggregates.Profile.Events;
using BuildingBlocks.Shared.IntegrationEvents.Users;

namespace Users.Application.Features.Profiles.Events;

public sealed class ProfileCreatedDomainEventHandler : INotificationHandler<ProfileCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProfileCreatedDomainEventHandler(
        IEventBus eventBus,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _eventBus = eventBus;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ProfileCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        // 1. Cria a notificação de boas-vindas para o usuário no sistema
        var welcomeNotification = Notification.Create(
            userId: notification.UserId,
            title: "Bem-vindo ao Btree!",
            message: $"Olá {notification.Name}, sua conta foi criada com sucesso.",
            notificationType: "System"
        );

        await _notificationRepository.AddAsync(welcomeNotification, cancellationToken);
        
        // 2. Publica o evento de integração no Outbox para outros módulos (ex: envio de e-mail de boas vindas)
        var integrationEvent = new UserRegisteredEvent
        {
            UserId = notification.UserId,
            Email = notification.Email,
            Name = notification.Name
        };

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        // 3. Salva a notificação (o _unitOfWork salva as mudanças no banco, e como o EventBus é o OutboxEventBus,
        // ele foi adicionado no mesmo DbContext e será salvo nesta transação).
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
